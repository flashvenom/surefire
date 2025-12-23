using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Quickfire.Blazor.Domain.Logs;
using Quickfire.Blazor.Domain.Users.Services;
using Quickfire.Blazor.Domain.Attachments.Models;
using Quickfire.Blazor.Domain.Shared.Helpers;

namespace Quickfire.Blazor.Domain.Ember
{
    public class EmberService
    {
        private readonly HubConnection _connection;
        private readonly UserService _userService;
        private readonly ILoggingService _log;
        private readonly Dictionary<string, Func<List<string>, Task>> _responseHandlers;

        public EmberService(UserService userService, ILoggingService loggingService)
        {
            _userService = userService;
            _log = loggingService;
            _responseHandlers = new Dictionary<string, Func<List<string>, Task>>();

            #if DEBUG
            string myurl = "https://localhost:7074/emberHub";
            #else
            string myurl = "https://surefire.local/emberHub";
            #endif
            
            _connection = new HubConnectionBuilder()
                .WithUrl(myurl, options =>
                {
                    // Increase buffer sizes for large data transfers
                    options.ApplicationMaxBufferSize = 1024 * 1024; // 1MB
                    options.TransportMaxBufferSize = 1024 * 1024; // 1MB
                })
                .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(30) })
                .Build();

            // Set up the response handler
            _connection.On<string, string, List<string>>("ReceiveEmberResponse", OnEmberResponse);
            
            // Set up connection event handlers
            _connection.Reconnecting += OnReconnecting;
            _connection.Reconnected += OnReconnected;
            _connection.Closed += OnClosed;
        }

        private async Task OnReconnecting(Exception? exception)
        {
            Console.WriteLine($"SignalR connection lost, attempting to reconnect... Error: {exception?.Message}");
            await _log.LogAsync(LogLevel.Warning, $"SignalR connection lost, attempting to reconnect: {exception?.Message}", "EmberService");
        }

        private async Task OnReconnected(string? connectionId)
        {
            Console.WriteLine($"SignalR connection reestablished with ID: {connectionId}");
            await _log.LogAsync(LogLevel.Information, $"SignalR connection reestablished: {connectionId}", "EmberService");
            
            // Rejoin the user group after reconnection
            try
            {
                var userId = await _userService.GetCurrentUserDesktopUsernameAsync();
                if (!string.IsNullOrEmpty(userId))
                {
                    await _connection.InvokeAsync("JoinGroup", userId);
                    Console.WriteLine($"Rejoined SignalR group for user: {userId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rejoining group after reconnection: {ex.Message}");
                await _log.LogAsync(LogLevel.Error, $"Error rejoining group after reconnection: {ex.Message}", "EmberService");
            }
        }

        private async Task OnClosed(Exception? exception)
        {
            Console.WriteLine($"SignalR connection closed. Error: {exception?.Message}");
            await _log.LogAsync(LogLevel.Warning, $"SignalR connection closed: {exception?.Message}", "EmberService");
        }

        // This method should be called to start the SignalR connection
        public async Task StartConnectionAsync(string? userIdOverride = null)
        {
            if (_connection.State == HubConnectionState.Disconnected)
            {
                try
                {
                    await _connection.StartAsync();
                    //Console.WriteLine("-------------SignalR connection started--------------------");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error starting SignalR connection: {ex.Message}");
                    await _log.LogAsync(LogLevel.Error, $"Error starting SignalR connection: {ex.Message}", "EmberService");
                }
            }

            if (_connection.State == HubConnectionState.Connected)
            {
                var userId = string.IsNullOrWhiteSpace(userIdOverride)
                    ? await _userService.GetCurrentUserDesktopUsernameAsync()
                    : userIdOverride;

                if (!string.IsNullOrEmpty(userId))
                {
                    await _connection.InvokeAsync("JoinGroup", userId);
                    Console.WriteLine($"Joined SignalR group for user: {userId}");
                }
            }
        }

        // Register a response handler for a specific command
        public void RegisterResponseHandler(string command, Func<List<string>, Task> handler)
        {
            _responseHandlers[command] = handler;
        }

        // Handle incoming responses from the Tray application
        private async Task OnEmberResponse(string userId, string command, List<string> responseData)
        {
            var dataSize = responseData?.Sum(d => d?.Length ?? 0) ?? 0;
            Console.WriteLine($"[EmberService] ========== OnEmberResponse START ==========");
            Console.WriteLine($"[EmberService] Received ember response for command: {command}");
            Console.WriteLine($"[EmberService] User ID: {userId}");
            Console.WriteLine($"[EmberService] Data size: {dataSize:N0} characters");
            Console.WriteLine($"[EmberService] Response data count: {responseData?.Count ?? 0}");
            Console.WriteLine($"[EmberService] Connection state: {_connection?.State}");
            
            if (responseData != null && responseData.Count > 0)
            {
                var firstItem = responseData[0];
                if (firstItem != null)
                {
                    var preview = firstItem.Length > 100 ? firstItem.Substring(0, 100) + "..." : firstItem;
                    Console.WriteLine($"[EmberService] First response preview: '{preview}'");
                }
            }
            
            if (_responseHandlers.TryGetValue(command, out var handler))
            {
                try
                {
                    Console.WriteLine($"[EmberService] Invoking handler for command: {command}");
                    await handler(responseData);
                    Console.WriteLine($"[EmberService] Successfully processed response for command: {command}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[EmberService] Error handling response for command {command}: {ex.Message}");
                    Console.WriteLine($"[EmberService] Exception type: {ex.GetType().Name}");
                    Console.WriteLine($"[EmberService] Stack trace: {ex.StackTrace}");
                    await _log.LogAsync(LogLevel.Error, $"Error handling response for command {command}: {ex.Message}", "EmberService");
                }
            }
            else
            {
                Console.WriteLine($"[EmberService] No handler registered for command: {command}");
                Console.WriteLine($"[EmberService] Available handlers: {string.Join(", ", _responseHandlers.Keys)}");
                await _log.LogAsync(LogLevel.Warning, $"No handler registered for command: {command}", "EmberService");
            }
            
            Console.WriteLine($"[EmberService] ========== OnEmberResponse END ==========");
        }

        // Send a command via SignalR
        public async Task RunEmberFunction(string emberFunction, List<string> parameters, string? userOverride = null)
        {
            Console.WriteLine($"[EmberService] ========== RunEmberFunction START ==========");
            Console.WriteLine($"[EmberService] Function: {emberFunction}");
            Console.WriteLine($"[EmberService] Parameters count: {parameters?.Count ?? 0}");
            var userId = await ResolveUserIdAsync(userOverride);
            Console.WriteLine($"[EmberService] SignalR Local User ID: {userId}");

            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("[EmberService] ERROR: User is not authenticated.");
                await _log.LogAsync(LogLevel.Error, "Error: User is not authenticated.", "EmberService");
                Console.WriteLine($"[EmberService] ========== RunEmberFunction END (Error) ==========");
                return;
            }

            await StartConnectionAsync(userId);

            Console.WriteLine($"[EmberService] Connection state before send: {_connection.State}");
            if (_connection.State != HubConnectionState.Connected)
            {
                Console.WriteLine("[EmberService] Connection not ready after start attempt.");
                await _log.LogAsync(LogLevel.Error, "SignalR connection not ready for Ember command.", "EmberService");
                Console.WriteLine($"[EmberService] ========== RunEmberFunction END (Error) ==========");
                return;
            }

            try
            {
                Console.WriteLine($"[EmberService] Sending SignalR command: {emberFunction}");
                Console.WriteLine($"[EmberService] Parameters: [{string.Join(", ", parameters ?? new List<string>())}]");
                
                await _connection.InvokeAsync("SendEmberCommand", userId, emberFunction, parameters);
                
                Console.WriteLine($"[EmberService] ✅ Successfully sent {emberFunction} command");
                Console.WriteLine($"[EmberService] ========== RunEmberFunction END (Success) ==========");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EmberService] ❌ SignalR error: {ex.Message}");
                Console.WriteLine($"[EmberService] Exception type: {ex.GetType().Name}");
                Console.WriteLine($"[EmberService] Connection state: {_connection.State}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[EmberService] Inner exception: {ex.InnerException.Message}");
                }
                Console.WriteLine($"[EmberService] ========== RunEmberFunction END (Error) ==========");
                await _log.LogAsync(LogLevel.Error, $"Error sending ember command: {ex.Message}", "EmberService");
            }
        }

        private async Task<string?> ResolveUserIdAsync(string? userOverride)
        {
            if (!string.IsNullOrWhiteSpace(userOverride))
            {
                return await _userService.GetDesktopUsernameForUserAsync(userOverride);
            }

            return await _userService.GetCurrentUserDesktopUsernameAsync();
        }

        public async Task WindowsOpenFile(Attachment attachment)
        {
            Console.WriteLine("Opening file...");
            List<string> mylistfiles = new List<string> { StringHelper.BuildWindowsPath(attachment, false) };
            await RunEmberFunction("Windows_OpenFile", mylistfiles);
        }

        public async Task WindowsOpenFolder(Attachment attachment)
        {
            Console.WriteLine("Opening folder...");
            if (!string.IsNullOrEmpty(attachment?.LocalPath))
            {
                List<string> mylistfiles = new List<string> { StringHelper.BuildWindowsPath(attachment, true) };
                await RunEmberFunction("Windows_OpenFolder", mylistfiles);
            }
        }

        // Clean up the connection on disposal
        public async ValueTask DisposeAsync()
        {
            if (_connection != null)
            {
                await _connection.DisposeAsync();
            }
        }
    }
}
