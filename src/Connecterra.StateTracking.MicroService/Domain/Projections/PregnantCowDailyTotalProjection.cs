using Connecterra.StateTracking.EventStore.Domain.Events;
using Connecterra.StateTracking.Persistent.Projections;
using System;

namespace Connecterra.StateTracking.MicroService.Projections
{
    public class PregnantCowDailyTotal
    {
        public int Count { get; set; } = 0;
    }

    public class PregnantCowDailyTotalProjection : CosmosProjection<PregnantCowDailyTotal>
    {
        public PregnantCowDailyTotalProjection()
        {
            RegisterHandler<CowPregnant>(WhenCowPregnant);
        }

        public override string GetViewName(DateTime timeStamp)
        {
            return Helper.GetPregnantCowViewName(timeStamp);
        }

        private void WhenCowPregnant(CowPregnant cowPregnant,
            PregnantCowDailyTotal view)
        {
            view.Count += 1;
        }

    }
}