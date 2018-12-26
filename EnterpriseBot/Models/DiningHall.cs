using Newtonsoft.Json;

namespace EnterpriseBot.Models
{
    public class DiningHall
    {
        [JsonProperty("Building")]
        public string Building;

        [JsonProperty("Id")]
        public string Id;

        [JsonProperty("Capability")]
        public double Capability;
    }
}
