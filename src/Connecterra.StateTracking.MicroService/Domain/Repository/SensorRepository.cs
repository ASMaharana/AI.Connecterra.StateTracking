using System.Linq;
using System.Threading.Tasks;
using Connecterra.StateTracking.Common.Core;
using Connecterra.StateTracking.Persistent.EventStore;

namespace Connecterra.StateTracking.MicroService.Domain.Repository
{
    public class SensorRepository : IRepository<Sensor>
    {
        private readonly IEventStore _eventStore;
        public SensorRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<Result<Sensor>> LoadAsync(string id)
        {
            var streamId = $"sensor:{id}";

            var result = await _eventStore.LoadStreamAsync(streamId);
            
            if(result.IsSuccess)
                return Result.OK(new Sensor(result.Value.Events));
            return Result.Error<Sensor>(result.ErrorMessage);
        }

        public async Task<Result> SaveAsync(Sensor sensor)
        {
            if (sensor.Changes.Any())
            {
                var streamId = $"sensor:{sensor.SensorId}";

                return await _eventStore.AppendToStreamAsync(
                    streamId,
                    sensor.Version,
                    sensor.Changes);
            }

            return Result.Error("Nothing is there to save");
        }
    }
}