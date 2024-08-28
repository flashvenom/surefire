using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Mantis.Domain.Voip;

namespace Mantis.Controllers
{
    [Route("ringcentral/webhook")]
    [ApiController]
    public class RingCentralWebhookController : ControllerBase
    {
        private readonly CallAlertService _callAlertService;

        public RingCentralWebhookController(CallAlertService callAlertService)
        {
            _callAlertService = callAlertService;
        }

        [HttpPost]
        public IActionResult HandleWebhook([FromBody] JObject data)
        {
            var callState = data["body"]["telephonyStatus"]?.ToString();

            if (callState == "Ringing")
            {
                var callerId = data["body"]["from"]["phoneNumber"]?.ToString();
                _callAlertService.NotifyIncomingCallAsync(callerId);
            }

            return Ok();
        }
    }
}
