using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Surefire.Domain.Users.Services;
using Microsoft.AspNetCore.Components.Web;
using Surefire.Domain.Logs;
using RingCentral;
using System.Security.Policy;



namespace Surefire.Domain.Ember
{
    public class EmberService
    {
        private readonly HubConnection _connection;
        private readonly UserService _userService;
        private readonly ILoggingService _log;

        public EmberService(UserService userService, ILoggingService loggingService)
        {
            _userService = userService;
            _log = loggingService;


#if DEBUG
                string myurl = "https://localhost:7074/emberHub";
#else
            string myurl = "https://bizname-web/emberHub";
#endif
            //string myurl = "https://bizname-web/emberHub";
            // Initialize the SignalR client connection
            _connection = new HubConnectionBuilder()
                .WithUrl(myurl)
                .Build();
        }

        // This method should be called to start the SignalR connection
        public async Task StartConnectionAsync()
        {
            if (_connection.State == HubConnectionState.Disconnected)
            {
                try
                {
                    await _connection.StartAsync();
                    Console.WriteLine("-------------SignalR connection started--------------------");
                    await _log.LogAsync(LogLevel.Information, "SignalR connection started.", "EmberService");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error starting SignalR connection: {ex.Message}");
                    await _log.LogAsync(LogLevel.Error, $"Error starting SignalR connection: {ex.Message}", "EmberService");
                }
            }
        }

        // Send a command via SignalR
        public async Task RunEmberFunction(string emberFunction, List<string> parameters)
        {
            Console.WriteLine("Getting");
            var phoneUserId = await _userService.GetCurrentUserDesktopUsernameAsync();
            var userId = phoneUserId;
            Console.WriteLine($"SignalR Local User ID: {userId}");

            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("Error: User is not authenticated.");
                await _log.LogAsync(LogLevel.Error, "Error: User is not authenticated.", "EmberService");
                return;
            }

            if (_connection.State != HubConnectionState.Connected)
            {
                Console.WriteLine("--Starting connection...");
                await StartConnectionAsync();  // Ensure the connection is started
                Console.WriteLine("--Started...");
            }

            try
            {
                await _connection.InvokeAsync("SendEmberCommand", userId, emberFunction, parameters);
                Console.WriteLine($"SignalR sending {emberFunction} command with parameters: {string.Join(", ", parameters)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR error: {ex.Message}");
                await _log.LogAsync(LogLevel.Error, $"Error sending ember command: {ex.Message}", "EmberService");
            }
        }

        //public async Task OpenFolder(string folderPath)
        //{
        //    Console.WriteLine($"SignalR opening local folder: {folderPath}");
        //    if (string.IsNullOrEmpty(folderPath)) return;

        //    var parameters = new List<string> { folderPath };
        //    await RunEmberFunction("OpenFolder", parameters);
        //}

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
