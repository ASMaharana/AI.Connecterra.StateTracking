using System.Threading.Tasks;
using Connecterra.StateTracking.Common.Core;
using Connecterra.StateTracking.Persistent.EventStore;
using Connecterra.StateTracking.SnapshotStore;

namespace Connecterra.StateTracking.MicroService.Domain.Repository
{
    public class CowRepositorySnapshotDecorator : IRepositorySnapshotDecorator<Cow>
    {
        private readonly IEventStore _eventStore;
        private readonly ISnapshotStore _snapshotStore;
        private readonly IRepository<Cow> _innerCowRepository;

        public CowRepositorySnapshotDecorator(IEventStore eventStore, 
            ISnapshotStore snapshotStore,
            IRepository<Cow> innerCowRepository)
        {
            _eventStore = eventStore;
            _snapshotStore = snapshotStore;
            _innerCowRepository = innerCowRepository;
        }

        public async Task<Result<Cow>> LoadAsync(string id)
        {
            var streamId = $"cow:{id}";

            var snapshotResult = await _snapshotStore.LoadSnapshotAsync(streamId);
            if (snapshotResult.IsSuccess)
            {
                var streamTailResult = await _eventStore.LoadStreamAsync(streamId, snapshotResult.Value.Version + 1);

                return streamTailResult.IsSuccess? Result.OK(new Cow(
                    snapshotResult.Value.SnapshotData.ToObject<CowSnapshot>(),
                    snapshotResult.Value.Version,
                    streamTailResult.Value.Events)) : Result.Error<Cow>(streamTailResult.ErrorMessage);
            }
            else
            {
                return await _innerCowRepository.LoadAsync(id);
            }
        }

        public Task<Result> SaveAsync(Cow cow)
        {
            return _innerCowRepository.SaveAsync(cow);
        }
    }
}