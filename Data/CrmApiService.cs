using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Mantis.Data;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

//If you happen to have an API to access "live" client data
namespace Mantis.Data
{
    public class CrmApiService
    {
        private readonly HttpClient _httpClient;
        private readonly CrmApiOptions _options;

        public CrmApiService(HttpClient httpClient, IOptions<CrmApiOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.ams.com/token");
            var credentials = $"{_options.ClientId}:{_options.ClientSecret}";
            var encodedCredentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(credentials));

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", encodedCredentials);
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" }
        });

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);
            return tokenResponse["access_token"];
        }

        public async Task<string> GetClientDetailsAsync(string lookupCode, string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.ams.com/token?client={lookupCode}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetClientPoliciesAsync(string clientId, string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.ams.com/clients/{clientId}/poli");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetPolicyLinesAsync(string ePolicyId, string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.ams.com/policies/{ePolicyId}/admin");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }

    public class CrmApiOptions
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}