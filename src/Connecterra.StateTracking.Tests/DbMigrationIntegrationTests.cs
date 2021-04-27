using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using Xunit;

namespace Connecterra.StateTracking.Tests
{
    public class DbMigrationIntegrationTests 
    {
        private const string EndpointUrl = "https://localhost:8081";
        private static readonly string AuthorizationKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private const string DatabaseId = "state-tracking-db";

        [Fact]
        public async Task Execute_MigrateDB_Success()
        {
            CosmosClient client = new CosmosClient(EndpointUrl, AuthorizationKey);
            
            await client.CreateDatabaseIfNotExistsAsync(DatabaseId, ThroughputProperties.CreateManualThroughput(400));
            Database database = client.GetDatabase(DatabaseId);

            await database.DefineContainer("events", "/stream/id").CreateIfNotExistsAsync();
            await database.DefineContainer("leases", "/id").CreateIfNotExistsAsync();
            await database.DefineContainer("views", "/id").CreateIfNotExistsAsync();
            await database.DefineContainer("snapshots", "/id").CreateIfNotExistsAsync();

            Container eventsContainer = database.GetContainer("events");
            await eventsContainer.Scripts.CreateStoredProcedureAsync(new StoredProcedureProperties
            {
                Id = "sp_appendtostream",
                Body = File.ReadAllText("EventStore/StoredProcedure/sp_appendtostream.js")
            });
        }
    }
}
