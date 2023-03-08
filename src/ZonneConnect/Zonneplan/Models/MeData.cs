using Newtonsoft.Json;

namespace ZonneConnect.Zonneplan.Models;

public class MeData
{
    [JsonProperty("address_groups")]
    public List<AddressGroup> AddressGroups = new List<AddressGroup>();
}

