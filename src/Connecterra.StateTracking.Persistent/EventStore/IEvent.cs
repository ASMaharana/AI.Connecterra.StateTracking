using System;

namespace Connecterra.StateTracking.Persistent.EventStore
{
    public interface IEvent
    {
        DateTime Timestamp { get; }
    }
}