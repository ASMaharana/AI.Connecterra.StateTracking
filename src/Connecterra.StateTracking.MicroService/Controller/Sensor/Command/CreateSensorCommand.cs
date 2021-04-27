using Connecterra.StateTracking.Common.Interface;
using Newtonsoft.Json;

namespace Connecterra.StateTracking.MicroService.Command
{
    public class CreateSensorCommand : ICommand
    {
        public string SensorId { get; }
        public string FarmId { get; }

        [JsonConstructor]
        public CreateSensorCommand(string sensorId, string farmId)
        {
            this.SensorId = sensorId;
            this.FarmId = farmId;
        }
    }
}
