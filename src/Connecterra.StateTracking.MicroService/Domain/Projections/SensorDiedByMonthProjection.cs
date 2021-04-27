using Connecterra.StateTracking.EventStore.Domain.Events;
using Connecterra.StateTracking.Persistent.Projections;
using System;

namespace Connecterra.StateTracking.MicroService.Projections
{
    public class SensorDiedByMonth
    {
        public int Count { get; set; } = 0;
    }

    public class SensorDiedByMonthProjection : CosmosProjection<SensorDiedByMonth>
    {
        public SensorDiedByMonthProjection()
        {
            RegisterHandler<SensorDead>(WhenSensorDead);
        }

        public override string GetViewName(DateTime timeStamp)
        {
            return Helper.GetDiedSensorViewName(timeStamp.Month, timeStamp.Year);
        }

        private void WhenSensorDead(SensorDead sensorDead,
            SensorDiedByMonth view)
        {
            view.Count += 1;
        }
    }
}