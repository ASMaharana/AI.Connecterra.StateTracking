using System.Threading.Tasks;
using Connecterra.StateTracking.EventStore.Domain.Events;
using Connecterra.StateTracking.MicroService;
using Connecterra.StateTracking.MicroService.Domain;
using Connecterra.StateTracking.MicroService.Domain.Repository;
using Connecterra.StateTracking.MicroService.Projections;
using Connecterra.StateTracking.Persistent.Connection;
using Connecterra.StateTracking.Persistent.EventStore;
using Connecterra.StateTracking.Persistent.Projections;
using Moq;
using Xunit;

namespace Connecterra.StateTracking.Tests
{
    public class CowStateTrackingIntegrationTests
    {
        private const string EndpointUrl = "https://localhost:8081";
        private static readonly string AuthorizationKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private const string DatabaseId = "state-tracking-db";
        
        [Fact]
        public async Task Execute_Cow_CreateStreamAsync()
        {
            var cowOpen = new CowOpen
            {
                CowId = "A",
                FarmId = "B",
            };

            var streamId = $"cow:{cowOpen.CowId}";
            var outcome = await GetEventStore().AppendToStreamAsync(
                streamId,
                0,
                new IEvent[] { cowOpen });

            Assert.True(outcome.IsSuccess, "Unexpected stream version encountered.");
        }

        [Fact]
        public async Task Execute_CowUpdateStreamAsync()
        {
            IEventStore eventStore = GetEventStore();

            var streamId = $"cow:A";
            var stream = await eventStore.LoadStreamAsync(streamId);

            var outcome = await eventStore.AppendToStreamAsync(
                streamId,
                stream.Value.Version,
                new IEvent[] { new CowInseminated() });

            Assert.True(outcome.IsSuccess, "Unexpected stream version encountered.");
        }

        [Fact]
        public async Task Execute_Cow_DomainAddAsync()
        {
            IEventStore eventStore = GetEventStore();
            var cow = new Cow("A", "B");
            cow.ChangeState(CowState.Pregnant);

            var repository = new CowRepository(eventStore);
            var outcome = await repository.SaveAsync(cow);

            Assert.True(outcome.IsSuccess, "Unexpected stream version encountered.");
        }

        [Fact]
        public async Task Execute_Cow_DomainUpdateAsync()
        {
            IEventStore eventStore = GetEventStore();

            // Request parameters.
            var cowId = "cow:A";

            var repository = new CowRepository(eventStore);
            var cow = await repository.LoadAsync(cowId);

            // Call business logic on domain object.
            cow.Value.ChangeState(CowState.Pregnant);

            var outcome = await repository.SaveAsync(cow.Value);
            Assert.True(outcome.IsSuccess, "Unexpected stream version encountered.");
        }

        [Fact]
        public async Task Run_ProjectionAsync()
        {
            IViewRepository viewRepository = new CosmosViewRepository(new CosmosDbClient(GetConfigurationLocator()));
            IProjectionEngine projectionEngine = new CosmosProjectionEngine(new EventTypeResolver(), viewRepository, new CosmosDbClient(GetConfigurationLocator()));

            projectionEngine.RegisterProjection(new PregnantCowDailyTotalProjection());

            await projectionEngine.StartAsync("TestInstance");
            await Task.Delay(-1);
        }

        private IEventStore GetEventStore()
        {
            return new CosmosEventStore(new CosmosDbClient(GetConfigurationLocator()), new EventTypeResolver());
        }

        private IConfigurationLocator GetConfigurationLocator()
        {
            var mockConfiguration = new Mock<IConfigurationLocator>();
            mockConfiguration.Setup(x => x.EndpointUrl)
                .Returns(EndpointUrl);
            mockConfiguration.Setup(x => x.AuthorizationKey)
                .Returns(AuthorizationKey);
            mockConfiguration.Setup(x => x.DatabaseId)
                .Returns(DatabaseId);
            mockConfiguration.Setup(x => x.EventContainerId)
                .Returns("events");
            return mockConfiguration.Object;
        }
    }
}
