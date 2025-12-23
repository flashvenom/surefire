using Microsoft.AspNetCore.SignalR.Client;
using System.IO;

/// <summary>
/// Simple exe that sends an incoming call notification to Surefire's SignalR hub.
/// </summary>
class SurefireCall
{
    private static readonly string LogFileName = "SurefireCall.log";
    private static readonly string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LogFileName);

    static async Task Main(string[] args)
    {
        if (args.Length < 1)
        {
            LogMessage("No caller ID provided. Exiting silently.");
            return;
        }

        string callerId = args[0]; // Phone number
        string callerName = args.Length >= 2 ? args[1] : "Unknown Caller";
        var callInfo = new CallInfo
        {
            CallerId = callerId,
            CallerName = callerName
        };

        LogMessage($"Processing incoming call from {callerName} ({callerId})");

        // Define the SignalR hub URLs
        //"https://localhost:7074/notificationHub"
        var hubUrls = new[]
        {
            "https://surefire.local/notificationHub"
        };

        // Create and start connections
        var hubConnections = new List<HubConnection>();

        foreach (var hubUrl in hubUrls)
        {
            try
            {
                var hubConnection = new HubConnectionBuilder()
                    .WithUrl(hubUrl)
                    .Build();

                hubConnections.Add(hubConnection);
                LogMessage($"Created connection to hub: {hubUrl}");
            }
            catch (Exception ex)
            {
                LogMessage($"Error creating connection to {hubUrl}: {ex.Message}");
            }
        }

        try
        {
            // Start all connections asynchronously
            var startTasks = hubConnections.Select(hubConnection => hubConnection.StartAsync());
            await Task.WhenAll(startTasks);
            LogMessage("All hub connections started successfully");

            // Send the message to all hubs asynchronously
            var sendTasks = hubConnections.Select(hubConnection => hubConnection.InvokeAsync("SendIncomingCall", callInfo));
            await Task.WhenAll(sendTasks);
            LogMessage("Call notification sent successfully to all hubs");
        }
        catch (Exception ex)
        {
            LogMessage($"Error during hub operations: {ex.Message}");
        }
        finally
        {
            // Dispose all connections
            var disposeTasks = hubConnections.Select(hubConnection => hubConnection.DisposeAsync().AsTask());
            await Task.WhenAll(disposeTasks);
            LogMessage("All connections disposed");
        }
    }

    private static void LogMessage(string message)
    {
        try
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var logMessage = $"[{timestamp}] {message}{Environment.NewLine}";
            File.AppendAllText(LogFilePath, logMessage);
        }
        catch (Exception ex)
        {
            // If logging fails, write to console as fallback
            Console.WriteLine($"Failed to write to log file: {ex.Message}");
            Console.WriteLine($"Original message: {message}");
        }
    }
}

public class CallInfo
{
    public string CallerId { get; set; }
    public string CallerName { get; set; }
}