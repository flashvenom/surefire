using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class CrmApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _clientId;
    private readonly string _clientSecret;

    public CrmApiService(HttpClient httpClient, string clientId, string clientSecret)
    {
        _httpClient = httpClient;
        _clientId = clientId;
        _clientSecret = clientSecret;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.***REMOVED***/v1/auth/connect/token");
        var credentials = $"{_clientId}:{_clientSecret}";
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
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.***REMOVED***/crm/v1/clients?lookupCode={lookupCode}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}
