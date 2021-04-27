using Connecterra.StateTracking.Common.Core;
using Connecterra.StateTracking.Common.Interface;
using Connecterra.StateTracking.MicroService.Domain;
using Connecterra.StateTracking.MicroService.Domain.Repository;
using System.Threading.Tasks;

namespace Connecterra.StateTracking.MicroService.Command
{
    public class CreateSensorCommandHandler : ICommandHandler<CreateSensorCommand>
    {
        private readonly IRepository<Sensor> _sensorRepository;

        public CreateSensorCommandHandler(IRepository<Sensor> sensorRepository)
        {
            _sensorRepository = sensorRepository;
        }
        
        public async Task<Result> HandleAsync(CreateSensorCommand command)
        {
            var sensor = new Sensor(command.SensorId, command.FarmId);
            var result = await _sensorRepository.SaveAsync(sensor);
            return result;
        }
    }
}
