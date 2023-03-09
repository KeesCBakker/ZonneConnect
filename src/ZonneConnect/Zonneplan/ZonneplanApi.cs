using Newtonsoft.Json;
using System.Net.Http.Headers;
using ZonneConnect.Zonneplan.Models;

namespace ZonneConnect.Zonneplan;

public class ZonneplanApi
{
    private const string API_VERSION = "2.1.1";
    private const string LOGIN_REQUEST_URI = "https://app-api.zonneplan.nl/auth/request";
    private const string OAUTH2_TOKEN_URI = "https://app-api.zonneplan.nl/oauth/token";
    private const string ME_URI = "https://app-api.zonneplan.nl/user-accounts/me";
    private const string BASE_URI = "https://app-api.zonneplan.nl";


    private readonly HttpClient _client;

    public ZonneplanApi(HttpClient httpClient)
    {
        _client = httpClient;
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _client.DefaultRequestHeaders.Add("x-app-version", API_VERSION);
    }

    public async Task<TempPassword> RequestTemporaryPassword(string email)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, LOGIN_REQUEST_URI);
        request.Content = new StringContent(JsonConvert.SerializeObject(new Request { Email = email }), System.Text.Encoding.UTF8, "application/json");
        var tempPassword = await Send<TempPassword>(request);
        return tempPassword!;
    }

    public async Task<Token?> GetTemporaryPassword(string email, string uuid)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, LOGIN_REQUEST_URI + "/" + uuid);
        var password = await Send<TempPassword>(request);
        if (password != null && password.IsActivated == true && password.Password != null)
        {
            var grantParams = new GrantParams { GrantType = "one_time_password", Email = email, Password = password.Password };
            return await GetNewToken(grantParams);
        }

        return null;
    }

    public async Task<Token?> RefreshToken(Token token)
    {
        var grantParams = new GrantParams { GrantType = "refresh_token", RefreshToken = token.RefreshToken };
        return await GetNewToken(grantParams);
    }

    private async Task<Token?> GetNewToken(GrantParams grantParams)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, OAUTH2_TOKEN_URI);
        request.Content = new StringContent(JsonConvert.SerializeObject(grantParams), System.Text.Encoding.UTF8, "application/json");
        using var response = await _client.SendAsync(request);
        var json = await response.Content.ReadAsStringAsync();
        var token = JsonConvert.DeserializeObject<Token>(json)!;
        return token;
    }

    public async Task<MeData?> Me(Token token)
    {
        return await Get<MeData>(token, ME_URI);
    }

    public async Task<T?> Get<T>(Token token, string url)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Authorization", token.TokenType + " " + token.AccessToken);
        return await Send<T>(request);
    }

    private async Task<T?> Send<T>(HttpRequestMessage request)
    {
        using var response = await _client.SendAsync(request);
        var json = await response.Content.ReadAsStringAsync();

        var apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(json, new JsonSerializerSettings
        {
            DateTimeZoneHandling = DateTimeZoneHandling.Utc
        })!;

        apiResponse.ThrowException();
        return apiResponse.Data;
    }

    public async Task<MeasurementsData[]> Current(Token token, string uuid)
    {
        var url = $"{BASE_URI}/connections/{uuid}/pv_installation/charts/live";
        var data = await Get<MeasurementsData[]>(token, url);
        return data!;
    }
}
