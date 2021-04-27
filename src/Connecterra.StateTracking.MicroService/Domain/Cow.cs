using Connecterra.StateTracking.Common.Core;
using Connecterra.StateTracking.EventStore.Domain.Events;
using Connecterra.StateTracking.Persistent.EventStore;
using System;
using System.Collections.Generic;

namespace Connecterra.StateTracking.MicroService.Domain
{
    public class Cow
    {
        public string CowId { get; private set; }
        public string FarmId { get; private set; }
        public CowState State { get; private set; }

        public int Version { get; set; }
        internal List<IEvent> Changes { get; }

        public Cow(string cowId, string farmId)
        {
            Changes = new List<IEvent>();
            
            Apply(new CowOpen
            {
                CowId = cowId,
                FarmId = farmId,
            });
        }

        public Cow(IEnumerable<IEvent> events)
        {
            Changes = new List<IEvent>();

            foreach (var @event in events)
            { 
                Mutate(@event);
                Version += 1;
            }
        }

        public Result ChangeState(CowState newState)
        {
            if(Math.Abs(newState-State) != 1)
            {
                return Result.Error("Invalid state");
            }
            switch(newState)
            {
                case CowState.Open:
                    Apply(new CowOpen
                    {
                        CowId = CowId,
                        FarmId = FarmId,
                    });
                    return Result.OK();
                case CowState.Inseminated:
                    Apply(new CowInseminated());
                    return Result.OK();
                case CowState.Pregnant:
                    Apply(new CowPregnant());
                    return Result.OK();
                case CowState.Dry:
                    Apply(new CowDry());
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

        private void When(CowOpen @event)
        {
            CowId = @event.CowId;
            FarmId = @event.FarmId;
            State = CowState.Open;
        }

        private void When(CowInseminated @event)
        {
            State = CowState.Inseminated;
        }

        private void When(CowPregnant @event)
        {
            State = CowState.Pregnant;
        }

        private void When(CowDry @event)
        {
            State = CowState.Dry;
        }

        public Cow(CowSnapshot snapshot, int version, IEnumerable<IEvent> events)
        {
            CowId = snapshot.CowId;
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

        public CowSnapshot GetSnapshot()
        {
            return new CowSnapshot(CowId, FarmId, State);
        }
    }
}