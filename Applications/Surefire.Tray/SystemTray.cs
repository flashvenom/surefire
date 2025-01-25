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


namespace SurefireTray
{
    public partial class SystemTray : Form
    {
        private HubConnection _connection;
        private string _userId;
        private string _logFilePath = @"C:\SUREFIRE\TrayLog.txt";

        public SystemTray()
        {
            InitializeComponent();

            // Hide the main window on startup
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.ControlBox = false;
            this.Text = string.Empty;
            this.FormBorderStyle = FormBorderStyle.None;

            // Configure the NotifyIcon
            SurefireEmberIcon.Icon = Properties.Resources.notify; // Replace with your icon
            SurefireEmberIcon.Visible = true;

            // Add "Clear Debug Log" to the context menu
            ToolStripMenuItem clearLogMenuItem = new ToolStripMenuItem("Clear Debug Log");
            clearLogMenuItem.Click += ClearDebugLog_Click;
            EmberContextMenu.Items.Insert(1, clearLogMenuItem); // Insert above "Exit"

            // Handle Context Menu events
            EmberContextMenu.Items[0].Click += ShowDebugConsole_Click; // "Show Debug Console"
            EmberContextMenu.Items[2].Click += Exit_Click; // "Exit"

            // Initialize user ID
            _userId = Environment.UserName;
        }

        private async void SystemTray_Load(object sender, EventArgs e)
        {
            // Start the SignalR connection
            Log("-----------Starting EmberApp------------");
            await StartSignalRConnection();
        }

        private async Task StartSignalRConnection()
        {
#if DEBUG
            string myurl = "https://localhost:7074/emberHub";
#else
            string myurl = "https://metro-web/emberHub";
#endif
            Log("     SignalR:   Started");
            Log($"       UserId:   {_userId}");
            Log($"          Url:   {myurl}");
            Log("----------------------------------------");

            _connection = new HubConnectionBuilder().WithUrl(myurl, options => 
            {
                options.HttpMessageHandlerFactory = _ => new HttpClientHandler { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
            }).Build();
           

            // Listen for incoming commands
            _connection.On<string, List<string>>("ReceiveEmberCommand", (emberFunction, parameters) =>
            {
                Log($"Received ember command: {emberFunction}");
                Log($"Parameters received: {string.Join(", ", parameters)}");

                // Call the appropriate function based on the emberFunction parameter
                OutlookControl.PerformOutlookFunction(emberFunction, parameters);
            });

            try
            {
                await _connection.StartAsync(); // Start the SignalR connection
                Log("SignalR connection started.");

                await _connection.InvokeAsync("JoinGroup", _userId);
                Log($"Joined SignalR group with userId: {_userId}");
            }
            catch (System.Exception ex)
            {
                Log($"Error starting SignalR connection: {ex.Message}");
            }
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

        private void Log(string message)
        {
            File.AppendAllText(_logFilePath, $"{DateTime.Now}: {message}\n");
        }

        private void ClearDebugLog_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(_logFilePath))
                {
                    File.WriteAllText(_logFilePath, string.Empty); // Clear the file
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

    }
}
