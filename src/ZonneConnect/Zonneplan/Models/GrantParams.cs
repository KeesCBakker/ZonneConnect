using Newtonsoft.Json;

namespace ZonneConnect.Zonneplan.Models;

public class GrantParams
{
    [JsonProperty("grant_type")]
    public string GrantType { get; set; } = string.Empty;

    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    [JsonProperty("password")]
    public string Password { get; set; } = string.Empty;

    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;
}
