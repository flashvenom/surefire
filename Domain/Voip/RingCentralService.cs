using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Mantis.Domain.Voip
{
    public class RingCentralService
    {
        private readonly HttpClient _httpClient;
        private readonly RingCentralOptions _options;

        public event Action<string> OnDebugMessage;

        public RingCentralService(HttpClient httpClient, IOptions<RingCentralOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        private void Debug(string message)
        {
            OnDebugMessage?.Invoke(message);
        }

        public async Task<string> GetAccessToken()
        {
            Debug("Starting token creation...");
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.ApiUrl}/restapi/oauth/token");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("client_id", _options.ClientId),
                new KeyValuePair<string, string>("client_secret", _options.ClientSecret),
                new KeyValuePair<string, string>("username", _options.AdminUsername),
                new KeyValuePair<string, string>("password", _options.AdminPassword),
                new KeyValuePair<string, string>("extension", _options.Account)
            });

            request.Content = content;

            Debug("Sending token request to: " + request.RequestUri);
            Debug("Awaiting server response...");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            Debug("Server responded with status: " + response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            var token = JObject.Parse(responseContent)["access_token"].ToString();

            Debug("Token created successfully.");
            return token;
        }

        public async Task CreateSubscriptionAsync(string webhookUrl)
        {
            var accessToken = await GetAccessToken();
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.ApiUrl}/restapi/v1.0/subscription");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var subscriptionPayload = new
            {
                eventFilters = new string[]
                {
                    "/restapi/v1.0/account/~/extension/~/telephony/sessions"
                },
                deliveryMode = new
                {
                    transportType = "WebHook",
                    address = webhookUrl
                }
            };

            request.Content = new StringContent(JsonConvert.SerializeObject(subscriptionPayload), Encoding.UTF8, "application/json");

            Debug("Sending subscription request to: " + request.RequestUri);
            Debug("Payload: " + JsonConvert.SerializeObject(subscriptionPayload));
            Debug("Awaiting server response...");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            Debug("Server responded with status: " + response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            Debug("Full server response: " + responseContent);
        }
    }
}
