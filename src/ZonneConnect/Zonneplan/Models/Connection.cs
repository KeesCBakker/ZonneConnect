using Newtonsoft.Json;

namespace ZonneConnect.Zonneplan.Models;

public class Connection
{
    [JsonProperty("uuid")]
    public string Uuid { get; set; } = String.Empty;

    [JsonProperty("contracts")]
    public List<Contract> Contracts { get; set; } = new List<Contract>();
}

