using Connecterra.StateTracking.Persistent.EventStore;
using System;

namespace Connecterra.StateTracking.MicroService.Domain.Events
{
    public abstract class EventBase : IEvent
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}