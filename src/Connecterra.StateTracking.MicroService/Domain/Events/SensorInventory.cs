using System;
using Connecterra.StateTracking.MicroService.Domain;
using Connecterra.StateTracking.MicroService.Domain.Events;

namespace Connecterra.StateTracking.EventStore.Domain.Events
{
    public class SensorInventory : EventBase
    {
        public string SensorId { get; set; }
        public string FarmId { get; set; }
    }
}