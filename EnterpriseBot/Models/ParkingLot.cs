using Newtonsoft.Json;

namespace EnterpriseBot.Models
{
    public class ParkingLot
    {
        [JsonProperty("Floor")]
        public string Floor { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("IsEmpty")]
        public bool IsEmpty { get; set; }
    }
}
