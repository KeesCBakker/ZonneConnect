using Newtonsoft.Json;

namespace ZonneConnect.Zonneplan.Models;

public class Contract
{
    [JsonProperty("uuid")]
    public string Uuid { get; set; } = String.Empty;

    [JsonProperty("label")]
    public string Label { get; set; } = String.Empty;

    [JsonProperty("type")]
    public string Type { get; set; } = String.Empty;

    [JsonProperty("start_date")]
    public DateTime StartDate { get; set; }

    [JsonProperty("end_date")]
    public DateTime EndDate { get; set; }

    [JsonProperty("meta")]
    public Meta Meta { get; set; } = new Meta();
}

