using Newtonsoft.Json;

namespace ZonneConnect.Zonneplan.Models;

public class Request
{
    [JsonProperty("email")]
    public string? Email { get; set; }
}
