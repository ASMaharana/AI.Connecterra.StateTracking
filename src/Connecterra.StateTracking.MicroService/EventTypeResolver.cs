using System;
using Connecterra.StateTracking.Persistent.EventStore;

namespace Connecterra.StateTracking.MicroService
{
    public class EventTypeResolver : IEventTypeResolver
    {
        public Type GetEventType(string typeName)
        {
            return Type.GetType($"Connecterra.StateTracking.EventStore.Domain.Events.{typeName}, Connecterra.StateTracking.MicroService");
        }
    }
}