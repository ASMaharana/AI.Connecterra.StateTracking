using Connecterra.StateTracking.Common.Interface;
using Connecterra.StateTracking.MicroService.Domain;
using Newtonsoft.Json;

namespace Connecterra.StateTracking.MicroService.Command
{
    public class UpdateSensorCommand : ICommand
    {
        public string SensorId { get; }
        public string FarmId { get; }
        public SensorState State { get; }

        [JsonConstructor]
        public UpdateSensorCommand(string sensorId, string farmId, SensorState state)
        {
            this.SensorId = sensorId;
            this.FarmId = farmId;
            this.State = state;
        }
    }
}
