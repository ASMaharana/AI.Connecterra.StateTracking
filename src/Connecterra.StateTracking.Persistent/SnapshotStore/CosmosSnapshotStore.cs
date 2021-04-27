using System;
using System.Net;
using System.Threading.Tasks;
using Connecterra.StateTracking.Common.Core;
using Connecterra.StateTracking.Persistent.Connection;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json.Linq;

namespace Connecterra.StateTracking.SnapshotStore
{
    public interface ISnapshotStore
    {
        Task<Result<Snapshot>> LoadSnapshotAsync(string streamId);
        Task<Result> SaveSnapshotAsync(string streamId, int version, object snapshot);
    }
    public class CosmosSnapshotStore : ISnapshotStore
    {
        private readonly IDbClient _dbClient;

        public CosmosSnapshotStore(IDbClient dbClient)
        {
            _dbClient = dbClient;
        }
        public async Task<Result<Snapshot>> LoadSnapshotAsync(string streamId)
        {
            try
            {
                var container = _dbClient.GetSnapshotContainer();
                var partitionKey = new PartitionKey(streamId);

                var response = await container.ReadItemAsync<Snapshot>(streamId, partitionKey);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return Result.OK(response.Resource);
                }

                throw new Exception();
            }
            catch (Exception ex)
            {
                return Result.Error<Snapshot>($"Fail to LoadSnapshot for streamId:{streamId}");
            }
        }

        public async Task<Result> SaveSnapshotAsync(string streamId, int version, object snapshot)
        {
            try
            {
                var container = _dbClient.GetSnapshotContainer();

                var partitionKey = new PartitionKey(streamId);

                await container.UpsertItemAsync(new Snapshot
                {
                    StreamId = streamId,
                    Version = version,
                    SnapshotData = JObject.FromObject(snapshot)
                }, partitionKey);

                return Result.OK();
            }
            catch (Exception ex)
            {
                return Result.Error($"Fail to SaveSnapshot for streamId:{streamId}");
            }
        }
    }
}