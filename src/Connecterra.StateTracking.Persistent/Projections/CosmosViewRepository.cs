using System;
using System.Net;
using System.Threading.Tasks;
using Connecterra.StateTracking.Common.Core;
using Connecterra.StateTracking.Persistent.Connection;
using Microsoft.Azure.Cosmos;

namespace Connecterra.StateTracking.Persistent.Projections
{
    public interface IViewRepository
    {
        Task<Result<CosmosView>> LoadViewAsync(string name);

        Task<Result> SaveViewAsync(string name, CosmosView view);
    }
    public class CosmosViewRepository : IViewRepository
    {
        private readonly IDbClient _dbClient;

        public CosmosViewRepository(IDbClient dbClient)
        {
            _dbClient = dbClient;
        }

        public async Task<Result<CosmosView>> LoadViewAsync(string name)
        {
            var container = _dbClient.GetViewContainer();
            var partitionKey = new PartitionKey(name);

            try
            {
                var response = await container.ReadItemAsync<CosmosView>(name, partitionKey);
                return Result.OK(response.Resource);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return Result.OK(new CosmosView());
            }
            catch (Exception ex)
            {
                return Result.Error<CosmosView>($"Fail to load view: {name}");
            }
        }

        public async Task<Result> SaveViewAsync(string name, CosmosView view)
        {
            var container = _dbClient.GetViewContainer();
            var partitionKey = new PartitionKey(name);

            var item = new
            {
                id = name,
                logicalCheckpoint = view.LogicalCheckpoint,
                payload = view.Payload
            };

            try
            {
                var result = await container.UpsertItemAsync(item, partitionKey, new ItemRequestOptions
                {
                    IfMatchEtag = view.Etag
                });

                return Result.OK();
            }
            catch (Exception ex)
            {
                return Result.Error($"Fail to save view: {name}");
            }
        }
    }
}