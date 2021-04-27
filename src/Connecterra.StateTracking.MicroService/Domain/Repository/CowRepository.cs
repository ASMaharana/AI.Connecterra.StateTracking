using System.Linq;
using System.Threading.Tasks;
using Connecterra.StateTracking.Common.Core;
using Connecterra.StateTracking.Persistent.EventStore;

namespace Connecterra.StateTracking.MicroService.Domain.Repository
{
    public class CowRepository : IRepository<Cow>
    {
        private readonly IEventStore _eventStore;
        public CowRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<Result<Cow>> LoadAsync(string id)
        {
            var streamId = $"cow:{id}";

            var result = await _eventStore.LoadStreamAsync(streamId);
            
            if(result.IsSuccess)
                return Result.OK(new Cow(result.Value.Events));
            return Result.Error<Cow>(result.ErrorMessage);
        }

        public async Task<Result> SaveAsync(Cow cow)
        {
            if (cow.Changes.Any())
            {
                var streamId = $"cow:{cow.CowId}";

                return await _eventStore.AppendToStreamAsync(
                    streamId,
                    cow.Version,
                    cow.Changes);
            }

            return Result.Error("Nothing is there to save");
        }
    }
}