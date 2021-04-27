using Newtonsoft.Json;

namespace Connecterra.StateTracking.Persistent.EventStore
{
    public class StreamInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }
    }
}