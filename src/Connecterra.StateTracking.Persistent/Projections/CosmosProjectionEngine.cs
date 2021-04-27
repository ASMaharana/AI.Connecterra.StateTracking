using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Connecterra.StateTracking.Persistent.Connection;
using Connecterra.StateTracking.Persistent.EventStore;
using Microsoft.Azure.Cosmos;

namespace Connecterra.StateTracking.Persistent.Projections
{
    public interface IProjectionEngine
    {
        void RegisterProjection(IProjection projection);
        Task StartAsync(string instanceName);
        Task StopAsync();
    }
    public class CosmosProjectionEngine : IProjectionEngine
    {
        private readonly IDbClient _dbClient;
        private readonly IEventTypeResolver _eventTypeResolver;
        private readonly IViewRepository _viewRepository;
        private readonly List<IProjection> _projections;
        private ChangeFeedProcessor _changeFeedProcessor;

        public CosmosProjectionEngine(IEventTypeResolver eventTypeResolver,
            IViewRepository viewRepository,
            IDbClient dbClient)
        {
            _eventTypeResolver = eventTypeResolver;
            _viewRepository = viewRepository;
            _dbClient = dbClient;
            _projections = new List<IProjection>();
        }
        public void RegisterProjection(IProjection projection) => _projections.Add(projection);

        public Task StartAsync(string instanceName)
        {
            var eventContainer = _dbClient.GetEventContainer();
            var leaseContainer = _dbClient.GetLeaseContainer();

            _changeFeedProcessor = eventContainer
                .GetChangeFeedProcessorBuilder<Change>("Projection", HandleChangesAsync)
                .WithInstanceName(instanceName)
                .WithLeaseContainer(leaseContainer)
                .Build();

            return _changeFeedProcessor.StartAsync();
        }

        public Task StopAsync() => _changeFeedProcessor.StopAsync();

        private async Task HandleChangesAsync(IReadOnlyCollection<Change> changes, CancellationToken cancellationToken)
        {
            foreach (var change in changes)
            {
                var @event = change.GetEvent(_eventTypeResolver);

                var subscribedProjections = _projections
                    .Where(projection => projection.IsSubscribedTo(@event));
                
                foreach (var projection in subscribedProjections)
                {
                    var viewName = projection.GetViewName(@event.Timestamp);

                    var handled = false;
                    while (!handled)
                    {
                        var viewResult = await _viewRepository.LoadViewAsync(viewName);
                        if (viewResult.Value.IsNewerThanCheckpoint(change))
                        {
                            projection.Apply(@event, viewResult.Value);

                            viewResult.Value.UpdateCheckpoint(change);

                            var saveResult = await _viewRepository.SaveViewAsync(viewName, viewResult.Value);
                            if (saveResult.IsSuccess)
                                handled = true;
                        }
                        else
                        {
                            handled = true;
                        }

                        if (!handled)
                        {
                            await Task.Delay(100);
                        }
                    }
                }
            }
        }
    }
}