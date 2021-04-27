using Connecterra.StateTracking.Common.Core;
using Connecterra.StateTracking.EventStore.Domain.Events;
using Connecterra.StateTracking.Persistent.EventStore;
using System;
using System.Collections.Generic;

namespace Connecterra.StateTracking.MicroService.Domain
{
    public class Sensor
    {
        public string SensorId { get; private set; }
        public string FarmId { get; private set; }
        public SensorState State { get; private set; }

        public int Version { get; set; }
        internal List<IEvent> Changes { get; }

        public Sensor(string sensorId, string farmId)
        {
            Changes = new List<IEvent>();
            
            Apply(new SensorInventory
            {
                SensorId = sensorId,
                FarmId = farmId,
            });
        }

        public Sensor(IEnumerable<IEvent> events)
        {
            Changes = new List<IEvent>();

            foreach (var @event in events)
            { 
                Mutate(@event);
                Version += 1;
            }
        }

        public Result ChangeState(SensorState newState)
        {
            if (Math.Abs(newState - State) != 1)
            {
                return Result.Error("Invalid state");
            }
            switch (newState)
            {
                case SensorState.Inventory:
                    Apply(new SensorInventory
                    {
                        SensorId = SensorId,
                        FarmId = FarmId,
                    });
                    return Result.OK();
                case SensorState.Deployed:
                    Apply(new SensorDeployed());
                    return Result.OK();
                case SensorState.FarmerTriage:
                    Apply(new SensorFarmerTriage());
                    return Result.OK();
                case SensorState.Returned:
                    Apply(new SensorReturned());
                    return Result.OK();
                case SensorState.Dead:
                    Apply(new SensorDead());
                    return Result.OK();
                case SensorState.Refurbished:
                    Apply(new SensorRefurbished());
                    return Result.OK();
                default:
                    return Result.Error("Invalid state");
            }

        }

        private void Apply(IEvent @event)
        {
            Changes.Add(@event);
            Mutate(@event);
        }

        private void Mutate(IEvent @event)
        {
            ((dynamic)this).When((dynamic)@event);
        }

        private void When(SensorInventory @event)
        {
            SensorId = @event.SensorId;
            FarmId = @event.FarmId;
            State = SensorState.Inventory;
        }

        private void When(SensorDeployed @event)
        {
            State = SensorState.Deployed;
        }

        private void When(SensorFarmerTriage @event)
        {
            State = SensorState.Inventory;
        }

        private void When(SensorReturned @event)
        {
            State = SensorState.Returned;
        }

        private void When(SensorDead @event)
        {
            State = SensorState.Dead;
        }

        private void When(SensorRefurbished @event)
        {
            State = SensorState.Refurbished;
        }

        public Sensor(SensorSnapshot snapshot, int version, IEnumerable<IEvent> events)
        {
            SensorId = snapshot.SensorId;
            FarmId = snapshot.FarmId;
            State = snapshot.State;
            Changes = new List<IEvent>();
            Version = version;

            foreach (var @event in events)
            { 
                Mutate(@event);
                Version += 1;
            }
        }

        public SensorSnapshot GetSnapshot()
        {
            return new SensorSnapshot(SensorId, FarmId, State);
        }
    }
}