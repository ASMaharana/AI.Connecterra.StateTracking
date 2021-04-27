using Connecterra.StateTracking.Persistent.EventStore;
using Newtonsoft.Json;

namespace Connecterra.StateTracking.Persistent.Projections
{
    public class Change : EventWrapper
    {
        [JsonProperty("_lsn")]
        public long LogicalSequenceNumber { get; set; }
    }
}