using Newtonsoft.Json;

namespace ZonneConnect.Zonneplan.Models
{
    public class MeasurementsData
    {
        public DateTime Date { get; set; }

        public int Total { get; set; }

        public List<Measurement> Measurements { get; set; }= new List<Measurement>();
    }

    public class Measurement
    {
        [JsonProperty("value")]
        public int Value { get; set; }

        [JsonProperty("measured_at")]
        public DateTime MeasuredAt { get; set; }
    }
}
