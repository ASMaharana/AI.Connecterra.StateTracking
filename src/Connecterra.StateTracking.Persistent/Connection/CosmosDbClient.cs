using Microsoft.Azure.Cosmos;
using System;

namespace Connecterra.StateTracking.Persistent.Connection
{
    public interface IDbClient
    {
        CosmosClient GetClient();
        Container GetLeaseContainer();
        Container GetEventContainer();
        Container GetViewContainer();
        Container GetSnapshotContainer();
    }
    public class CosmosDbClient : IDbClient
    {
        private readonly IConfigurationLocator _configuration;
        public CosmosDbClient(IConfigurationLocator configuration)
        {
            _configuration = configuration;
        }
        public CosmosClient GetClient() => new CosmosClient(_configuration.EndpointUrl, _configuration.AuthorizationKey);
        public Container GetLeaseContainer() => GetClient().GetContainer(_configuration.DatabaseId, 
            _configuration.LeaseContainerId);
        public Container GetEventContainer() => GetClient().GetContainer(_configuration.DatabaseId,
            _configuration.EventContainerId);
        public Container GetViewContainer() => GetClient().GetContainer(_configuration.DatabaseId,
            _configuration.ViewContainerId);
        public Container GetSnapshotContainer() => GetClient().GetContainer(_configuration.DatabaseId,
            _configuration.SnapshotContainerId);
    }
}