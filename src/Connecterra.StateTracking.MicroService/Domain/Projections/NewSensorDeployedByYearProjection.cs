using Connecterra.StateTracking.EventStore.Domain.Events;
using Connecterra.StateTracking.Persistent.Projections;
using System;
using System.Collections.Generic;

namespace Connecterra.StateTracking.MicroService.Projections
{
    public class NewSensorDeployedByYear
    {
        public Dictionary<int, int> Deployed { get; } = new Dictionary<int, int>();
    }

    public class NewSensorDeployedByYearProjection : CosmosProjection<NewSensorDeployedByYear>
    {
        public NewSensorDeployedByYearProjection()
        {
            RegisterHandler<SensorDeployed>(WhenSensorDeployed);
        }

        public override string GetViewName(DateTime timeStamp)
        {
            return Helper.GetNewSensorViewName(timeStamp.Year);
        }

        private void WhenSensorDeployed(SensorDeployed sensorDeployed,
            NewSensorDeployedByYear view)
        {
            var key = sensorDeployed.Timestamp.Date.Month;
            if (view.Deployed.TryGetValue(key, out var count))
            {
                view.Deployed[key] = count + 1;
            }
            else
            {
                view.Deployed.Add(key, 1);
            }
        }
    }
}