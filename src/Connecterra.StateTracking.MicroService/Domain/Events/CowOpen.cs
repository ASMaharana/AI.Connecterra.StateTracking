using Connecterra.StateTracking.MicroService.Domain.Events;

namespace Connecterra.StateTracking.EventStore.Domain.Events
{
    public class CowOpen : EventBase
    {
        public string CowId { get; set; }
        public string FarmId { get; set; }
    }
}