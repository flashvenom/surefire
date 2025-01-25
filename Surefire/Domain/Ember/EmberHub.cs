using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Surefire.Domain.Ember
{
    public class EmberHub : Hub
    {
        // Method for clients to join a group based on user ID
        public async Task JoinGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }

        // Send command to a specific user group
        public async Task SendEmberCommand(string userId, string emberFunction, List<string> parameters)
        {
            Console.WriteLine($"Sending {userId} the command {emberFunction} with {parameters.Count} parameters.");
            // Send ember command to the specific user's group
            await Clients.Group(userId).SendAsync("ReceiveEmberCommand", emberFunction, parameters);
            Console.WriteLine("Sent");
        }
    }
}
