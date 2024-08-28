using Microsoft.AspNetCore.SignalR;

namespace Mantis.Domain.Voip
{
    public class CallAlertService
    {
        private readonly IHubContext<CallAlertHub> _hubContext;

        public CallAlertService(IHubContext<CallAlertHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyIncomingCallAsync(string callerId)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveCallAlert", callerId);
        }
    }
}
