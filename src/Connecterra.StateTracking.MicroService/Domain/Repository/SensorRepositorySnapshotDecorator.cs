using System.Threading.Tasks;
using Connecterra.StateTracking.Common.Core;
using Connecterra.StateTracking.Persistent.EventStore;
using Connecterra.StateTracking.SnapshotStore;

namespace Connecterra.StateTracking.MicroService.Domain.Repository
{
    public class SensorRepositorySnapshotDecorator : IRepositorySnapshotDecorator<Sensor>
    {
        private readonly IEventStore _eventStore;
        private readonly ISnapshotStore _snapshotStore;
        private readonly IRepository<Sensor> _innerSensorRepository;

        public SensorRepositorySnapshotDecorator(IEventStore eventStore, 
            ISnapshotStore snapshotStore,
            IRepository<Sensor> innerSensorRepository)
        {
            _eventStore = eventStore;
            _snapshotStore = snapshotStore;
            _innerSensorRepository = innerSensorRepository;
        }

        public async Task<Result<Sensor>> LoadAsync(string id)
        {
            var streamId = $"sensor:{id}";

            var snapshotResult = await _snapshotStore.LoadSnapshotAsync(streamId);
            if (snapshotResult.IsSuccess)
            {
                var streamTailResult = await _eventStore.LoadStreamAsync(streamId, snapshotResult.Value.Version + 1);

                return streamTailResult.IsSuccess ? Result.OK(new Sensor(
                    snapshotResult.Value.SnapshotData.ToObject<SensorSnapshot>(),
                    snapshotResult.Value.Version,
                    streamTailResult.Value.Events)) : Result.Error<Sensor>(streamTailResult.ErrorMessage);
            }
            else
            {
                return await _innerSensorRepository.LoadAsync(id);
            }
        }

        public Task<Result> SaveAsync(Sensor sensor)
        {
            return _innerSensorRepository.SaveAsync(sensor);
        }
    }
}