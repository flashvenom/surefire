using Microsoft.AspNetCore.SignalR;

namespace Mantis.Domain.Voip
{
    public class CallAlertHub : Hub
    {
    }

    public class RingCentralOptions
    {
        public string ApiUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Account { get; set; }
        public string AdminUsername { get; set; }
        public string AdminPassword { get; set; }
    }
}
