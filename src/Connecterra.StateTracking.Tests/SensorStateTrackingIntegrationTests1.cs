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
    public class SensorStateTrackingIntegrationTests
    {
        private const string EndpointUrl = "https://localhost:8081";
        private static readonly string AuthorizationKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private const string DatabaseId = "state-tracking-db";
        
        [Fact]
        public async Task Execute_Sensor_CreateStreamAsync()
        {
            var sensorInventory = new SensorInventory
            {
                SensorId = "A",
                FarmId = "B",
            };

            var streamId = $"sensor:{sensorInventory.SensorId}";
            var outcome = await GetEventStore().AppendToStreamAsync(
                streamId,
                0,
                new IEvent[] { sensorInventory });

            Assert.True(outcome.IsSuccess, "Unexpected stream version encountered.");
        }

        [Fact]
        public async Task Execute_SensorUpdateStreamAsync()
        {
            IEventStore eventStore = GetEventStore();

            var streamId = $"sensor:A";
            var stream = await eventStore.LoadStreamAsync(streamId);

            var outcome = await eventStore.AppendToStreamAsync(
                streamId,
                stream.Value.Version,
                new IEvent[] { new SensorDeployed() });

            Assert.True(outcome.IsSuccess, "Unexpected stream version encountered.");
        }

        [Fact]
        public async Task Execute_Sensor_DomainAddAsync()
        {
            IEventStore eventStore = GetEventStore();
            var sensor = new Sensor("A", "B");

            var repository = new SensorRepository(eventStore);
            var outcome = await repository.SaveAsync(sensor);

            Assert.True(outcome.IsSuccess, "Unexpected stream version encountered.");
        }

        [Fact]
        public async Task Execute_Sensor_DomainUpdateAsync()
        {
            IEventStore eventStore = GetEventStore();

            // Request parameters.
            var sensorId = "sensor:A";

            var repository = new SensorRepository(eventStore);
            var sensor = await repository.LoadAsync(sensorId);

            // Call business logic on domain object.
            sensor.Value.ChangeState(SensorState.Deployed);

            var outcome = await repository.SaveAsync(sensor.Value);
            Assert.True(outcome.IsSuccess, "Unexpected stream version encountered.");
        }

        [Fact]
        public async Task Run_ProjectionAsync()
        {
            IViewRepository viewRepository = new CosmosViewRepository(new CosmosDbClient(GetConfigurationLocator()));
            IProjectionEngine projectionEngine = new CosmosProjectionEngine(new EventTypeResolver(), viewRepository, new CosmosDbClient(GetConfigurationLocator()));

            projectionEngine.RegisterProjection(new NewSensorDeployedByYearProjection());
            projectionEngine.RegisterProjection(new SensorDiedByMonthProjection());

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
