using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Office.Interop.Outlook;
using System.Net.Http;
using System.Drawing;

namespace Quickfire.Tray
{
    public partial class SystemTray : Form
    {
        private HubConnection _connection;
        private string _userId;
        private static readonly string LogFilePath = Path.Combine(AppContext.BaseDirectory, "TrayLog.txt");
        
        // Notification tracking
        private static Dictionary<string, DateTime> _recentNotifications = new Dictionary<string, DateTime>();
        private static object _notificationLock = new object();
        private static int _notificationCooldown = 30; // Changed from 5 to 30 seconds
        
        // Connection status tracking
        private bool _isConnected = false;
        private Timer _reconnectTimer;
        private int _reconnectAttempt = 0;
        private readonly int[] _reconnectIntervals = { 10, 10, 10, 30, 60, 120, 300, 3000, 10000 }; // Seconds between reconnect attempts
        private ToolStripMenuItem _reconnectMenuItem;
        private ToolStripMenuItem _statusMenuItem;
        private Timer _connectionCheckTimer;
        private bool _isLocalhostMode = false;
        private ToolStripMenuItem _localhostModeMenuItem;

        // Icons for different connection states
        private Icon _connectedIcon;
        private Icon _disconnectedIcon;

        // Log file management
        private static int _logWriteCounter = 0;
        private static readonly object _logLock = new object();
        private static readonly long _maxLogFileSize = 1024 * 1024; // 1MB
        private static readonly long _trimToSize = 512 * 1024; // Trim down to 512KB when limit exceeded

        public SystemTray()
        {
            InitializeComponent();

            // Load icons
            _connectedIcon = Properties.Resources.notify;
            _disconnectedIcon = Properties.Resources.notify; // Replace with a different icon if available

            // Hide the main window on startup
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.ControlBox = false;
            this.Text = string.Empty;
            this.FormBorderStyle = FormBorderStyle.None;

            // Configure the NotifyIcon
            SurefireEmberIcon.Icon = _connectedIcon;
            SurefireEmberIcon.Visible = true;
            SurefireEmberIcon.Text = "Quickfire Tray - Initializing...";

            // Setup context menu
            SetupContextMenu();

            // Initialize user ID
            _userId = Environment.UserName;
            
            // Initialize reconnect timer
            _reconnectTimer = new Timer();
            _reconnectTimer.Tick += ReconnectTimer_Tick;
            
            // Initialize connection check timer - this is now mainly a safety net since we have proper event handling
            _connectionCheckTimer = new Timer();
            _connectionCheckTimer.Interval = 300000; // Check every 5 minutes (reduced frequency since events handle most cases)
            _connectionCheckTimer.Tick += ConnectionCheckTimer_Tick;
            _connectionCheckTimer.Start();
            
            // Register for system events
            System.Windows.Forms.Application.ApplicationExit += Application_ApplicationExit;
        }

        private void SetupContextMenu()
        {
            // Start with a clean slate – clear designer-added items to avoid duplicates
            EmberContextMenu.Items.Clear();

            // Add status indicator to the context menu (first item)
            _statusMenuItem = new ToolStripMenuItem("Status: Initializing...");
            _statusMenuItem.Enabled = false;
            EmberContextMenu.Items.Insert(0, _statusMenuItem);
            
            // Add separator
            EmberContextMenu.Items.Insert(1, new ToolStripSeparator());

            // Add "Reconnect" item with keyboard shortcut
            _reconnectMenuItem = new ToolStripMenuItem("Reconnect to Server", null, Reconnect_Click, Keys.Control | Keys.R);
            EmberContextMenu.Items.Insert(2, _reconnectMenuItem);
            _reconnectMenuItem.Enabled = false;
            
            // Add "Localhost Mode" toggle
            _localhostModeMenuItem = new ToolStripMenuItem("Localhost Mode");
            _localhostModeMenuItem.CheckOnClick = true;
            _localhostModeMenuItem.Click += LocalhostMode_Click;
            EmberContextMenu.Items.Insert(3, _localhostModeMenuItem);
            
            // Add "Show Debug Console" item
            ToolStripMenuItem consoleMenuItem = new ToolStripMenuItem("Show Debug Console", null, ShowDebugConsole_Click, Keys.Control | Keys.D);
            EmberContextMenu.Items.Insert(4, consoleMenuItem);

            // Add "Clear Debug Log" item
            ToolStripMenuItem clearLogMenuItem = new ToolStripMenuItem("Clear Debug Log");
            clearLogMenuItem.Click += ClearDebugLog_Click;
            EmberContextMenu.Items.Insert(5, clearLogMenuItem);
            
            // Add separator
            EmberContextMenu.Items.Insert(6, new ToolStripSeparator());

            // Keep the "Exit" item as last
            ToolStripMenuItem exitMenuItem = new ToolStripMenuItem("Exit", null, Exit_Click, Keys.Control | Keys.Q);
            EmberContextMenu.Items.Insert(7, exitMenuItem);
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            // Clean up resources when application exits
            try
            {
                Log("Application exiting - cleaning up resources");
                
                // Stop timers
                if (_reconnectTimer != null)
                {
                    _reconnectTimer.Stop();
                    _reconnectTimer.Dispose();
                }
                
                if (_connectionCheckTimer != null)
                {
                    _connectionCheckTimer.Stop();
                    _connectionCheckTimer.Dispose();
                }
                
                // Close SignalR connection
                if (_connection != null)
                {
                    Log("Closing SignalR connection");
                    _connection.StopAsync().Wait(2000); // Give it 2 seconds to close gracefully
                    _connection.DisposeAsync().AsTask().Wait(1000);
                }
                
                Log("Application cleanup completed");
            }
            catch (System.Exception ex)
            {
                Log($"Error during application cleanup: {ex.Message}");
                // Continue with shutdown even if cleanup fails
            }
        }

        private async void SystemTray_Load(object sender, EventArgs e)
        {
            // Check and trim log file on startup if needed
            TrimLogFileIfNeeded();
            
            Log("=========================================================");
            Log("-------------------- OPENFIRE TRAY!----------------------");
            Log($"[SystemTray] Application startup at: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
            Log($"[SystemTray] User: {Environment.UserName}");
            Log($"[SystemTray] Machine: {Environment.MachineName}");
            Log($"[SystemTray] OS: {Environment.OSVersion}");
            Log($"[SystemTray] .NET Version: {Environment.Version}");
            Log($"[SystemTray] Working Directory: {Environment.CurrentDirectory}");
            Log($"[SystemTray] Available Word command: GetWordDocContents");
            await StartSignalRConnection();
        }

        private async Task StartSignalRConnection()
        {
            string myurl = _isLocalhostMode ? "https://localhost:7074/emberHub" : "https://surefire.local/emberHub";

            Log("=========================================================");
            Log("-------------------- OPENFIRE TRAY ----------------------");
            Log($"     SignalR:   Starting connection to {myurl}");
            Log($"       UserId:   {_userId}");
            Log($"       Auth:     Checking authentication state...");

            _connection = new HubConnectionBuilder()
                .WithUrl(myurl, options =>
                {
                    options.HttpMessageHandlerFactory = _ => new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };
                    // Increase buffer sizes for large data transfers (like Word documents)
                    options.ApplicationMaxBufferSize = 10 * 1024 * 1024; // 10MB
                    options.TransportMaxBufferSize = 10 * 1024 * 1024; // 10MB
                })
                .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(30) })
                .Build();

            // Wire up connection event handlers - THIS WAS MISSING!
            _connection.Closed += Connection_Closed;
            _connection.Reconnecting += Connection_Reconnecting;
            _connection.Reconnected += Connection_Reconnected;

            // Listen for incoming commands
            _connection.On<string, List<string>>("ReceiveEmberCommand", (emberFunction, parameters) =>
            {
                Log($"[SystemTray] ========== ReceiveEmberCommand START ==========");
                Log($"[SystemTray] Received ember command: '{emberFunction}'");
                Log($"[SystemTray] Parameters count: {parameters?.Count ?? 0}");
                if (parameters != null && parameters.Count > 0)
                {
                    Log($"[SystemTray] Parameters: [{string.Join(", ", parameters.Select(p => $"'{p}'"))}]");
                }
                Log($"[SystemTray] Connection state: {_connection.State}");
                Log($"[SystemTray] Is authenticated: {_connection.State == HubConnectionState.Connected}");
                Log($"[SystemTray] Current user ID: '{_userId}'");
                Log($"[SystemTray] Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");

                try
                {
                    Log($"[SystemTray] Routing command to appropriate handler...");
                    
                    // Check and call appropriate functions based on the emberFunction prefix
                    if (emberFunction.ToLower().StartsWith("outlooksearch_"))
                    {
                        Log($"[SystemTray] Routing to OutlookControl (OutlookSearch_ prefix)");
                        OutlookControl.PerformOutlookFunction(emberFunction, parameters);
                    }
                    else if (emberFunction.ToLower().StartsWith("windows_"))
                    {
                        Log($"[SystemTray] Routing to WindowsControl (Windows_ prefix)");
                        WindowsControl.PerformWindowsFunction(emberFunction, parameters);
                    }
                    else if (emberFunction.ToLower().Equals("getworddoccontents"))
                    {
                        Log($"[SystemTray] Routing to WordControl (GetWordDocContents command)");
                        WordControl.PerformWordFunction(emberFunction, parameters);
                    }
                    else if (emberFunction.ToLower().Equals("showtraynotification"))
                    {
                        Log($"[SystemTray] Routing to ShowTrayNotification");
                        ShowTrayNotification(parameters);
                    }
                    else if (emberFunction.ToLower().Equals("showstaffchat"))
                    {
                        Log($"[SystemTray] Routing to ShowStaffChat");
                        ShowStaffChatNotification(parameters);
                    }
                    else
                    {
                        Log($"[SystemTray] No specific handler found, using fallback to OutlookControl");
                        Log($"[SystemTray] Available commands: OutlookSearch_*, Windows_*, GetWordDocContents, ShowTrayNotification");
                        // Fallback to try the general handler
                        OutlookControl.PerformOutlookFunction(emberFunction, parameters);
                    }
                    
                    Log($"[SystemTray] Command handler completed successfully");
                    Log($"[SystemTray] ========== ReceiveEmberCommand END (Success) ==========");
                }
                catch (System.Exception ex)
                {
                    Log($"[SystemTray] ========== ReceiveEmberCommand ERROR ==========");
                    Log($"[SystemTray] Exception Type: {ex.GetType().Name}");
                    Log($"[SystemTray] Exception Message: {ex.Message}");
                    Log($"[SystemTray] Stack Trace: {ex.StackTrace}");
                    
                    if (ex.InnerException != null)
                    {
                        Log($"[SystemTray] Inner Exception: {ex.InnerException.Message}");
                    }
                    
                    Log($"[SystemTray] Failed command: '{emberFunction}'");
                    Log($"[SystemTray] ========== ReceiveEmberCommand END (Exception) ==========");
                }
            });

            try
            {
                await _connection.StartAsync();
                _isConnected = true;
                _reconnectAttempt = 0;
                
                // Stop any running reconnect timer since we're now connected
                if (_reconnectTimer.Enabled)
                {
                    _reconnectTimer.Stop();
                }
                
                _reconnectMenuItem.Enabled = false;
                
                UpdateConnectionStatus("Connected", true);
                Log("     Status: Connected");
                Log($"Connection state: {_connection.State}");
                Log($"Authentication state: {_connection.State == HubConnectionState.Connected}");

                await _connection.InvokeAsync("JoinGroup", _userId);
                Log("       Join: Success!");
                Log("=========================================================");
            }
            catch (System.Exception ex)
            {
                _isConnected = false;
                _reconnectMenuItem.Enabled = true;
                
                UpdateConnectionStatus("Disconnected", false);
                Log($"Error starting SignalR connection: {ex.Message}");
                Log($"Connection state: {_connection.State}");
                Log($"Authentication state: {_connection.State == HubConnectionState.Connected}");
                Log($"Stack trace: {ex.StackTrace}");
                ShowReconnectNotification();
                
                // Don't start manual reconnect timer - let SignalR's automatic reconnection handle it
                // StartReconnectTimer();
            }
        }

        private Task Connection_Closed(System.Exception arg)
        {
            _isConnected = false;
            _reconnectMenuItem.Enabled = true;
            
            UpdateConnectionStatus("Disconnected", false);
            Log($"SignalR connection closed: {arg?.Message ?? "Unknown reason"}");
            
            // Don't show notification or start manual timer - SignalR will handle automatic reconnection
            // Only show notification if automatic reconnection completely fails
            
            return Task.CompletedTask;
        }

        private Task Connection_Reconnecting(System.Exception arg)
        {
            _isConnected = false;
            UpdateConnectionStatus("Reconnecting...", false);
            Log($"SignalR attempting to reconnect: {arg?.Message ?? "Unknown reason"}");
            return Task.CompletedTask;
        }

        private async Task Connection_Reconnected(string arg)
        {
            _isConnected = true;
            _reconnectAttempt = 0;
            _reconnectMenuItem.Enabled = false;
            
            // Stop any manual reconnect timer that might be running
            if (_reconnectTimer.Enabled)
            {
                _reconnectTimer.Stop();
            }
            
            UpdateConnectionStatus("Connected", true);
            Log($"SignalR reconnected with connection ID: {arg}");
            
            try
            {
                await _connection.InvokeAsync("JoinGroup", _userId);
                Log("Rejoined SignalR group after reconnection");
                
                // Clear any reconnection notifications and show success
                SurefireEmberIcon.BalloonTipText = "";
                ShowConnectedNotification();
            }
            catch (System.Exception ex)
            {
                Log($"Error rejoining group after reconnection: {ex.Message}");
                // Even if rejoin fails, the connection is still active, so don't mark as disconnected
            }
        }

        private void UpdateConnectionStatus(string status, bool isConnected)
        {
            // Update tray icon and menu
            _statusMenuItem.Text = $"Status: {status}";
            SurefireEmberIcon.Icon = isConnected ? _connectedIcon : _disconnectedIcon;
            SurefireEmberIcon.Text = $"Quickfire Tray - {status}";
            
            // Force refresh of the icon
            SurefireEmberIcon.Visible = false;
            SurefireEmberIcon.Visible = true;
        }

        private void StartReconnectTimer()
        {
            // This timer is only used as a fallback when SignalR's automatic reconnection has completely given up
            // Get the appropriate interval based on the number of attempts
            int intervalIndex = Math.Min(_reconnectAttempt, _reconnectIntervals.Length - 1);
            int intervalSeconds = _reconnectIntervals[intervalIndex];
            
            Log($"Starting fallback reconnection timer - attempt #{_reconnectAttempt + 1} in {intervalSeconds} seconds");
            Log("Note: This is a fallback mechanism. SignalR should handle most reconnections automatically.");
            
            _reconnectTimer.Interval = intervalSeconds * 1000;
            _reconnectTimer.Start();
        }

        private async void ReconnectTimer_Tick(object sender, EventArgs e)
        {
            _reconnectTimer.Stop();
            
            // Only attempt reconnection if we're still disconnected and SignalR isn't trying to reconnect
            if (!_isConnected && _connection?.State == HubConnectionState.Disconnected)
            {
                _reconnectAttempt++;
                Log($"Fallback reconnection attempt #{_reconnectAttempt} (SignalR automatic reconnection has stopped)");
                await TryReconnect();
            }
            else
            {
                Log($"Skipping fallback reconnection - Current state: IsConnected={_isConnected}, SignalR State={_connection?.State}");
            }
        }

        private void ConnectionCheckTimer_Tick(object sender, EventArgs e)
        {
            // Check if our internal state doesn't match the actual connection state
            if (_connection != null)
            {
                var actualState = _connection.State;
                
                // Log current states for debugging
                Log($"Connection check - Internal state: {(_isConnected ? "Connected" : "Disconnected")}, SignalR state: {actualState}");
                
                // If we think we're connected but SignalR says we're disconnected
                if (_isConnected && actualState == HubConnectionState.Disconnected)
                {
                    Log("Connection check detected disconnection - updating internal state");
                    _isConnected = false;
                    _reconnectMenuItem.Enabled = true;
                    UpdateConnectionStatus("Disconnected", false);
                    
                    // Show notification only if this is a newly detected disconnection
                    ShowReconnectNotification();
                    
                    // Don't start manual reconnection timer immediately - give SignalR's automatic 
                    // reconnection some time to work first (it may be in the process of reconnecting)
                }
                // If we think we're disconnected but SignalR says we're connected
                else if (!_isConnected && actualState == HubConnectionState.Connected)
                {
                    Log("Connection check detected reconnection - updating internal state");
                    _isConnected = true;
                    _reconnectMenuItem.Enabled = false;
                    UpdateConnectionStatus("Connected", true);
                    
                    // Try to rejoin the group to ensure we're properly connected
                    Task.Run(async () =>
                    {
                        try
                        {
                            await _connection.InvokeAsync("JoinGroup", _userId);
                            Log("Rejoined SignalR group after connection check detected reconnection");
                        }
                        catch (System.Exception ex)
                        {
                            Log($"Failed to rejoin group after connection check: {ex.Message}");
                        }
                    });
                }
                // If SignalR is in a transitional state, just update our UI to match
                else if (actualState == HubConnectionState.Connecting || actualState == HubConnectionState.Reconnecting)
                {
                    if (_isConnected)
                    {
                        Log($"Connection check detected transitional state: {actualState}");
                        _isConnected = false;
                        UpdateConnectionStatus("Reconnecting...", false);
                    }
                }
            }
        }

        private async void Reconnect_Click(object sender, EventArgs e)
        {
            if (!_isConnected)
            {
                _reconnectAttempt = 0; // Reset the counter for manual reconnection
                Log("Manual reconnection requested");
                await TryReconnect();
            }
        }

        private async Task TryReconnect()
        {
            try
            {
                Log($"Manual reconnection attempt - Current state: {_connection?.State ?? Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Disconnected}");
                UpdateConnectionStatus("Reconnecting...", false);
                
                if (_connection == null)
                {
                    Log("Connection is null, creating new connection");
                    await StartSignalRConnection();
                    return;
                }
                
                // Handle different connection states
                switch (_connection.State)
                {
                    case HubConnectionState.Disconnected:
                        Log("Connection is disconnected, attempting to start");
                        await _connection.StartAsync();
                        break;
                        
                    case HubConnectionState.Connecting:
                    case HubConnectionState.Reconnecting:
                        Log("Connection is already attempting to connect, waiting...");
                        // Don't interfere with ongoing connection attempt
                        return;
                        
                    case HubConnectionState.Connected:
                        Log("Connection is already connected, verifying group membership");
                        // Connection claims to be active, try to rejoin group to verify
                        await _connection.InvokeAsync("JoinGroup", _userId);
                        break;
                        
                    default:
                        Log($"Unknown connection state: {_connection.State}, forcing restart");
                        await _connection.StopAsync();
                        await _connection.StartAsync();
                        break;
                }
                
                // If we get here without exception, consider it successful
                _isConnected = true;
                _reconnectMenuItem.Enabled = false;
                
                UpdateConnectionStatus("Connected", true);
                Log("Manual reconnection successful");
                
                // Ensure we're in the group
                if (_connection.State == HubConnectionState.Connected)
                {
                    await _connection.InvokeAsync("JoinGroup", _userId);
                    Log("Rejoined SignalR group after manual reconnection");
                }
                
                // Clear notification and show success
                SurefireEmberIcon.BalloonTipText = "";
                ShowConnectedNotification();
            }
            catch (System.Exception ex)
            {
                _isConnected = false;
                UpdateConnectionStatus("Disconnected", false);
                Log($"Manual reconnection failed: {ex.Message}");
                Log($"Stack trace: {ex.StackTrace}");
                
                // Only start manual reconnect timer if SignalR's automatic reconnection has given up
                if (_connection?.State == HubConnectionState.Disconnected)
                {
                    Log("Starting manual reconnect timer as fallback");
                    StartReconnectTimer();
                }
            }
        }

        private void ShowReconnectNotification()
        {
            SurefireEmberIcon.BalloonTipTitle = "Connection Lost";
            SurefireEmberIcon.BalloonTipText = "Connection to server lost. Attempting to reconnect...";
            SurefireEmberIcon.BalloonTipIcon = ToolTipIcon.Warning;
            SurefireEmberIcon.ShowBalloonTip(5000);
            
            // Remove any existing click handlers to avoid conflicts
            SurefireEmberIcon.BalloonTipClicked -= OnReconnectBalloonTipClicked;
            SurefireEmberIcon.Click -= OnTrayIconClick;
            
            // Add click handler for manual reconnection if automatic fails
            SurefireEmberIcon.BalloonTipClicked += OnReconnectBalloonTipClicked;
            SurefireEmberIcon.Click += OnTrayIconClick;
        }
        
        private async void OnReconnectBalloonTipClicked(object sender, EventArgs e)
        {
            if (!_isConnected)
            {
                Log("User clicked reconnection notification");
                await TryReconnect();
            }
        }
        
        private async void OnTrayIconClick(object sender, EventArgs e)
        {
            if (!_isConnected)
            {
                Log("User clicked tray icon while disconnected");
                await TryReconnect();
            }
        }
        
        private void ShowConnectedNotification()
        {
            SurefireEmberIcon.BalloonTipTitle = "Connection Restored";
            SurefireEmberIcon.BalloonTipText = "Successfully reconnected to the server.";
            SurefireEmberIcon.BalloonTipIcon = ToolTipIcon.Info;
            SurefireEmberIcon.ShowBalloonTip(3000);
        }

        private void ShowDebugConsole_Click(object sender, EventArgs e)
        {
            ConsoleWindow.ToggleConsoleWindow();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            SurefireEmberIcon.Visible = false;
            System.Windows.Forms.Application.Exit();
        }

        private void ClearDebugLog_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(LogFilePath))
                {
                    lock (_logLock)
                    {
                        File.WriteAllText(LogFilePath, string.Empty); // Clear the file
                        _logWriteCounter = 0; // Reset the counter since file is now empty
                    }
                    MessageBox.Show("Debug log cleared successfully.", "Log Cleared", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No debug log file found to clear.", "Log Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Failed to clear the debug log. Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Log(string message)
        {
            lock (_logLock)
            {
                try
                {
                    var logDirectory = Path.GetDirectoryName(LogFilePath);
                    if (!string.IsNullOrWhiteSpace(logDirectory))
                    {
                        Directory.CreateDirectory(logDirectory);
                    }

                    string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}: {message}\n";
                    File.AppendAllText(LogFilePath, logEntry);
                    
                    // Check file size every 25 log writes to avoid performance impact
                    _logWriteCounter++;
                    if (_logWriteCounter >= 25)
                    {
                        _logWriteCounter = 0;
                        TrimLogFileIfNeeded();
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"Failed to write to log: {ex.Message}");
                }
            }
        }

        private static void TrimLogFileIfNeeded()
        {
            try
            {
                var fileInfo = new FileInfo(LogFilePath);
                if (!fileInfo.Exists || fileInfo.Length <= _maxLogFileSize)
                {
                    return; // File doesn't exist or is within size limit
                }

                // File is too large, trim it
                string[] allLines = File.ReadAllLines(LogFilePath);
                
                // Calculate how many characters we want to keep (approximately)
                long currentSize = fileInfo.Length;
                double keepRatio = (double)_trimToSize / currentSize;
                int linesToKeep = Math.Max(100, (int)(allLines.Length * keepRatio)); // Keep at least 100 lines
                
                // Keep the most recent lines
                string[] linesToWrite = allLines.Skip(allLines.Length - linesToKeep).ToArray();
                
                // Add a marker to show that the log was trimmed
                var trimmedContent = new List<string>();
                trimmedContent.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}: ========== LOG FILE TRIMMED (was {currentSize:N0} bytes, kept {linesToKeep:N0} most recent lines) ==========");
                trimmedContent.AddRange(linesToWrite);
                
                // Write the trimmed content back to the file
                File.WriteAllLines(LogFilePath, trimmedContent);
                
                Console.WriteLine($"Log file trimmed from {currentSize:N0} bytes to approximately {_trimToSize:N0} bytes");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Failed to trim log file: {ex.Message}");
                // Don't fail logging just because we couldn't trim
            }
        }

        public static async void SendEmberResponse(string command, List<string> responseData)
        {
            Log($"[SystemTray] ========== SendEmberResponse START ==========");
            Log($"[SystemTray] Command: '{command}'");
            Log($"[SystemTray] Response data count: {responseData?.Count ?? 0}");
            
            if (responseData != null && responseData.Count > 0)
            {
                for (int i = 0; i < responseData.Count; i++)
                {
                    var data = responseData[i];
                    if (data != null)
                    {
                        // Log first 200 characters of each response item for debugging
                        string preview = data.Length > 200 ? data.Substring(0, 200) + "..." : data;
                        Log($"[SystemTray] Response[{i}] length: {data.Length}, preview: '{preview.Replace('\r', ' ').Replace('\n', ' ')}'");
                    }
                    else
                    {
                        Log($"[SystemTray] Response[{i}] is null");
                    }
                }
            }
            
            try
            {
                Log($"[SystemTray] Step 1: Finding SystemTray form instance...");
                
                // Find the SystemTray instance
                var forms = System.Windows.Forms.Application.OpenForms;
                Log($"[SystemTray] Total open forms: {forms.Count}");
                
                SystemTray systemTrayForm = null;
                foreach (System.Windows.Forms.Form form in forms)
                {
                    Log($"[SystemTray] Checking form: {form.GetType().Name}");
                    if (form is SystemTray)
                    {
                        systemTrayForm = (SystemTray)form;
                        Log($"[SystemTray] Found SystemTray form instance");
                        break;
                    }
                }

                if (systemTrayForm == null)
                {
                    Log($"[SystemTray] ERROR: Could not find SystemTray form instance for sending response");
                    Log($"[SystemTray] Available forms: {string.Join(", ", forms.Cast<System.Windows.Forms.Form>().Select(f => f.GetType().Name))}");
                    Log($"[SystemTray] ========== SendEmberResponse END (No form) ==========");
                    return;
                }

                Log($"[SystemTray] Step 2: Checking SignalR connection...");
                Log($"[SystemTray] Connection state: {systemTrayForm._connection?.State ?? Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Disconnected}");
                Log($"[SystemTray] UserId: '{systemTrayForm._userId ?? "null"}'");

                if (systemTrayForm._connection == null)
                {
                    Log($"[SystemTray] ERROR: SignalR connection is null");
                    Log($"[SystemTray] ========== SendEmberResponse END (No connection) ==========");
                    return;
                }

                if (string.IsNullOrEmpty(systemTrayForm._userId))
                {
                    Log($"[SystemTray] ERROR: UserId is null or empty");
                    Log($"[SystemTray] ========== SendEmberResponse END (No userId) ==========");
                    return;
                }

                if (systemTrayForm._connection.State != Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected)
                {
                    Log($"[SystemTray] WARNING: SignalR connection is not in Connected state: {systemTrayForm._connection.State}");
                    Log($"[SystemTray] Attempting to send anyway...");
                }

                Log($"[SystemTray] Step 3: Sending response via SignalR...");
                Log($"[SystemTray] Hub method: 'SendEmberResponse'");
                Log($"[SystemTray] Parameters: userId='{systemTrayForm._userId}', command='{command}', responseData.Count={responseData?.Count ?? 0}");

                // Send the response back to the server
                await systemTrayForm._connection.InvokeAsync("SendEmberResponse", systemTrayForm._userId, command, responseData);
                
                Log($"[SystemTray] SUCCESS: Response sent successfully for command: {command}");
                Log($"[SystemTray] ========== SendEmberResponse END (Success) ==========");
            }
            catch (System.Exception ex)
            {
                Log($"[SystemTray] ========== SendEmberResponse ERROR ==========");
                Log($"[SystemTray] Exception Type: {ex.GetType().Name}");
                Log($"[SystemTray] Exception Message: {ex.Message}");
                Log($"[SystemTray] Stack Trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Log($"[SystemTray] Inner Exception: {ex.InnerException.Message}");
                }
                
                Log($"[SystemTray] Command that failed: '{command}'");
                Log($"[SystemTray] ========== SendEmberResponse END (Exception) ==========");
            }
        }

        private void ShowTrayNotification(List<string> parameters)
        {
            try
            {
                if (parameters == null || parameters.Count < 2)
                {
                    Log("Invalid parameters for ShowTrayNotification. Expected at least title and message.");
                    return;
                }

                string title = parameters[0];
                string message = parameters[1];
                ToolTipIcon icon = ToolTipIcon.None; // Set to None to prevent sound
                string clientId = null;

                // If we have a third parameter, it's the ClientId
                if (parameters.Count > 2)
                {
                    clientId = parameters[2];
                    Log($"Call notification for ClientId: {clientId}");
                    
                    // If we have a fourth parameter, it's the icon type
                    if (parameters.Count > 3)
                    {
                        if (Enum.TryParse<ToolTipIcon>(parameters[3], true, out ToolTipIcon parsedIcon))
                        {
                            icon = parsedIcon;
                        }
                    }
                }
                
                // Create a more robust notification key that handles null/empty values better
                // Include timestamp in seconds to ensure very similar notifications within a few milliseconds are still shown
                // Format: title|message|clientId|timestamp (using | as separator which is unlikely to be in the strings)
                string timestamp = DateTime.Now.ToString("HH:mm:ss");
                string notificationKey = $"{title}|{message}|{clientId ?? "none"}";
                
                Log($"Processing notification with key: {notificationKey}");
                
                bool isDuplicate = false;
                
                // Check if this is a duplicate notification
                lock(_notificationLock)
                {
                    // Clean up expired notifications
                    var expiredKeys = _recentNotifications.Where(kv => 
                        (DateTime.Now - kv.Value).TotalSeconds > _notificationCooldown)
                        .Select(kv => kv.Key).ToList();
                        
                    foreach(var key in expiredKeys)
                    {
                        _recentNotifications.Remove(key);
                        Log($"Removed expired notification: {key}");
                    }
                    
                    // Check if this notification was recently shown
                    if (_recentNotifications.ContainsKey(notificationKey))
                    {
                        isDuplicate = true;
                        Log($"Skipping duplicate notification: {notificationKey} (shown {(DateTime.Now - _recentNotifications[notificationKey]).TotalSeconds:0.00} seconds ago)");
                    }
                    else
                    {
                        // Add to recent notifications
                        _recentNotifications[notificationKey] = DateTime.Now;
                        Log($"Added new notification to tracking: {notificationKey}");
                    }
                }
                
                // Return early if it's a duplicate
                if (isDuplicate)
                {
                    Log($"Duplicate... skipping.");
                    return;
                    
                }

                // Remove any existing click handlers
                SurefireEmberIcon.BalloonTipClicked -= OnBalloonTipClicked;
                
                // Store the clientId for the click handler
                SurefireEmberIcon.Tag = clientId;

                SurefireEmberIcon.BalloonTipTitle = title;
                SurefireEmberIcon.BalloonTipText = message;
                SurefireEmberIcon.BalloonTipIcon = icon;

                // Add click handler if we have a ClientId
                if (!string.IsNullOrEmpty(clientId))
                {
                    SurefireEmberIcon.BalloonTipClicked += OnBalloonTipClicked;
                }

                SurefireEmberIcon.ShowBalloonTip(5000); // Show for 5 seconds

                Log($"Showing tray notification - Title: {title}, Message: {message}, Icon: {icon}");
            }
            catch (System.Exception ex)
            {
                Log($"Error showing tray notification: {ex.Message}");
            }
        }

        private void ShowStaffChatNotification(List<string> parameters)
        {
            try
            {
                if (parameters == null || parameters.Count < 3)
                {
                    Log("Invalid parameters for ShowStaffChatNotification. Expected at least sender name, message, and sender full name.");
                    return;
                }

                string senderName = parameters[0];
                string message = parameters[1];
                string senderFullName = parameters[2];

                string title = $"Staff Message from {senderFullName}";
                string notificationMessage = $"{senderName}: {message}";

                // Create notification key for cooldown (handled by main app, but log for debugging)
                Log($"Staff chat notification - From: {senderFullName} ({senderName}), Message: {message}");

                // Remove any existing click handlers
                SurefireEmberIcon.BalloonTipClicked -= OnStaffChatBalloonTipClicked;
                
                // Set special tag to indicate this is a staff chat notification
                SurefireEmberIcon.Tag = "STAFF_CHAT";

                SurefireEmberIcon.BalloonTipTitle = title;
                SurefireEmberIcon.BalloonTipText = notificationMessage;
                SurefireEmberIcon.BalloonTipIcon = ToolTipIcon.Info;

                // Add click handler for staff chat
                SurefireEmberIcon.BalloonTipClicked += OnStaffChatBalloonTipClicked;

                SurefireEmberIcon.ShowBalloonTip(8000); // Show for 8 seconds

                Log($"Showing staff chat notification - Title: {title}, Message: {notificationMessage}");
            }
            catch (System.Exception ex)
            {
                Log($"Error showing staff chat notification: {ex.Message}");
            }
        }

        private void OnStaffChatBalloonTipClicked(object sender, EventArgs e)
        {
            try
            {
                Log("Staff chat notification clicked - attempting to bring Surefire window to front");
                
                // Try to find and bring the Surefire Edge app window to front
                var surefireProcess = System.Diagnostics.Process.GetProcesses()
                    .FirstOrDefault(p => 
                        p.MainWindowTitle.ToLower().Contains("surefire") ||
                        p.ProcessName.ToLower().Contains("msedge") && 
                        p.MainWindowTitle.ToLower().Contains("surefire"));
                
                if (surefireProcess != null)
                {
                    Log($"Found Surefire window: {surefireProcess.MainWindowTitle} (Process: {surefireProcess.ProcessName})");
                    WindowsControl.BringWindowToFront(surefireProcess.MainWindowHandle);
                }
                else
                {
                    // Fallback - try to find any Edge process with a window title containing relevant terms
                    var edgeProcesses = System.Diagnostics.Process.GetProcesses()
                        .Where(p => p.ProcessName.ToLower().Contains("msedge") && 
                                   !string.IsNullOrEmpty(p.MainWindowTitle))
                        .ToList();
                    
                    Log($"Found {edgeProcesses.Count} Edge processes with windows");
                    foreach (var proc in edgeProcesses)
                    {
                        Log($"Edge process: {proc.MainWindowTitle}");
                    }
                    
                    // Try to find one that might be Surefire
                    var likelyProcess = edgeProcesses
                        .FirstOrDefault(p => p.MainWindowTitle.ToLower().Contains("surefire") ||
                                           p.MainWindowTitle.ToLower().Contains("localhost"));
                    
                    if (likelyProcess != null)
                    {
                        Log($"Found likely Surefire process: {likelyProcess.MainWindowTitle}");
                        WindowsControl.BringWindowToFront(likelyProcess.MainWindowHandle);
                    }
                    else
                    {
                        // Last resort - open Surefire URL
                        string surefireUrl = "https://surefire.local/";
                        Log($"Surefire window not found, opening URL: {surefireUrl}");
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = surefireUrl,
                            UseShellExecute = true
                        });
                    }
                }
            }
            catch (System.Exception ex)
            {
                Log($"Error handling staff chat notification click: {ex.Message}");
            }
        }

        private void OnBalloonTipClicked(object sender, EventArgs e)
        {
            try
            {
                // Get the clientId from the tag
                string clientId = SurefireEmberIcon.Tag as string;
                
                if (string.IsNullOrEmpty(clientId))
                {
                    Log("No clientId found for notification click");
                    return;
                }
                
                // First try to find and bring the Edge app window to front
                var edgeProcess = System.Diagnostics.Process.GetProcesses()
                    .FirstOrDefault(p => p.MainWindowTitle.ToLower().Equals("surefire"));
                
                if (edgeProcess != null)
                {
                    Log($"Found Edge app window: {edgeProcess.MainWindowTitle}");
                    WindowsControl.BringWindowToFront(edgeProcess.MainWindowHandle);
                }
                else
                {
                    // Fallback to opening URL if window not found
                    string clientUrl = $"https://surefire.local/Clients/{clientId}";
                    Log($"Edge app window not found, opening URL: {clientUrl}");
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = clientUrl,
                        UseShellExecute = true
                    });
                }
            }
            catch (System.Exception ex)
            {
                Log($"Error handling notification click: {ex.Message}");
            }
        }

        private async void LocalhostMode_Click(object sender, EventArgs e)
        {
            _isLocalhostMode = _localhostModeMenuItem.Checked;
            Log($"Localhost mode {( _isLocalhostMode ? "enabled" : "disabled" )}");
            
            // Disconnect current connection
            if (_connection != null)
            {
                try
                {
                    await _connection.StopAsync();
                }
                catch
                {
                    // Ignore errors during disconnect
                }
            }
            
            // Start new connection with updated URL
            await StartSignalRConnection();
        }
    }
}
