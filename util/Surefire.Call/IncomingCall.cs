using Microsoft.AspNetCore.SignalR.Client;

/// <summary>
/// Simple exe that sends an incoming call notification to Surefire's SignalR hub.
/// </summary>
class SurefireCall
{
    static async Task Main(string[] args)
    {
        if (args.Length < 1)
        {
            // Exit silently if no caller ID is provided
            return;
        }

        string callerId = args[0]; // Phone number
        string callerName = args.Length >= 2 ? args[1] : "Unknown Caller";
        var callInfo = new CallInfo
        {
            CallerId = callerId,
            CallerName = callerName
        };

        // Define the SignalR hub URLs
        var hubUrls = new[]
        {
            "https://bizname-web/notificationHub",
            "https://localhost:7074/notificationHub"
        };

        // Create and start connections
        var hubConnections = new List<HubConnection>();

        foreach (var hubUrl in hubUrls)
        {
            var hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .Build();

            hubConnections.Add(hubConnection);
        }

        // Start all connections asynchronously
        var startTasks = hubConnections.Select(hubConnection => hubConnection.StartAsync());
        await Task.WhenAll(startTasks);

        // Send the message to all hubs asynchronously
        var sendTasks = hubConnections.Select(hubConnection => hubConnection.InvokeAsync("SendIncomingCall", callInfo));
        await Task.WhenAll(sendTasks);
       
        // Dispose all connections
        var disposeTasks = hubConnections.Select(hubConnection => hubConnection.DisposeAsync().AsTask());
        await Task.WhenAll(disposeTasks);
    }
}

public class CallInfo
{
    public string CallerId { get; set; }
    public string CallerName { get; set; }
}