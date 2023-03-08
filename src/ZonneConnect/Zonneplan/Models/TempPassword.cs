using Newtonsoft.Json;

namespace ZonneConnect.Zonneplan.Models;

public class TempPassword
{
    [JsonProperty("uuid")]
    public string Uuid { get; set; } = string.Empty;

    [JsonProperty("is_activated")]
    public bool IsActivated { get; set; }

    [JsonProperty("password")]
    public string Password { get; set; } = string.Empty;
}
