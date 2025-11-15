using Microsoft.AspNetCore.SignalR;
using Surefire.Data;

public class NotificationHub : Hub
{
    // Method to handle incoming call notifications
    public async Task SendIncomingCall(CallInfo callInfo)
    {
        Console.Write("Incoming call detected");
        // Broadcast the caller ID to all connected clients
        await Clients.All.SendAsync("ReceiveCallNotification", callInfo);
    }
}