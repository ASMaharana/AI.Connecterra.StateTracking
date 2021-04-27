using System;

namespace Connecterra.StateTracking.Persistent.EventStore
{
    public interface IEventTypeResolver
    {
        Type GetEventType(string typeName);
    }
}