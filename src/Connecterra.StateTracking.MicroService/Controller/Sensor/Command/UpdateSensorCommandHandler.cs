using Connecterra.StateTracking.Common.Core;
using Connecterra.StateTracking.Common.Interface;
using Connecterra.StateTracking.MicroService.Domain;
using Connecterra.StateTracking.MicroService.Domain.Repository;
using System.Threading.Tasks;

namespace Connecterra.StateTracking.MicroService.Command
{
    public class UpdateSensorCommandHandler : ICommandHandler<UpdateSensorCommand>
    {
        private readonly IRepositorySnapshotDecorator<Sensor> _sensorRepository;

        public UpdateSensorCommandHandler(IRepositorySnapshotDecorator<Sensor> sensorRepository)
        {
            _sensorRepository = sensorRepository;
        }
        
        public async Task<Result> HandleAsync(UpdateSensorCommand command)
        {
            var getResult = await _sensorRepository.LoadAsync(command.SensorId);
            if (getResult.IsSuccess)
            {
                var changeStateResult = getResult.Value.ChangeState(command.State);
                if (changeStateResult.IsSuccess)
                {
                    var saveResult = await _sensorRepository.SaveAsync(getResult.Value);
                    return saveResult;
                }
                return changeStateResult;
            }
            return Result.Error(getResult.ErrorMessage);
        }
    }
}
