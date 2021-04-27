using System;
using Newtonsoft.Json;

namespace Connecterra.StateTracking.MicroService.Domain
{
    public class CowSnapshot
    {
        public CowSnapshot(string cowId, string farmId, CowState state)
        {
            CowId = cowId;
            FarmId = farmId;
            State = state;
        }

        [JsonConstructor]
        private CowSnapshot()
        {
        }

        [JsonProperty("cowId")]
        public string CowId { get; private set; }

        [JsonProperty("farmId")]
        public string FarmId { get; private set; }

        [JsonProperty("state")]
        public CowState State { get; private set; }
    }
}