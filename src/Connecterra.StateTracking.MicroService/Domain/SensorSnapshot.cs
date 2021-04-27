using Newtonsoft.Json;

namespace Connecterra.StateTracking.MicroService.Domain
{
    public class SensorSnapshot
    {
        public SensorSnapshot(string sensorId, string farmId, SensorState state)
        {
            SensorId = sensorId;
            FarmId = farmId;
            State = state;
        }

        [JsonConstructor]
        private SensorSnapshot()
        {
        }

        [JsonProperty("sensorId")]
        public string SensorId { get; private set; }

        [JsonProperty("farmId")]
        public string FarmId { get; private set; }

        [JsonProperty("state")]
        public SensorState State { get; private set; }
    }
}