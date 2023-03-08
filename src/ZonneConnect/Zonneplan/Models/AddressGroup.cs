using Newtonsoft.Json;

namespace ZonneConnect.Zonneplan.Models;

public class AddressGroup
{
    [JsonProperty("uuid")]
    public string Uuid { get; set; } = String.Empty;

    [JsonProperty("connections")]
    public List<Connection> Connections { get; set; } = new List<Connection>();
}

