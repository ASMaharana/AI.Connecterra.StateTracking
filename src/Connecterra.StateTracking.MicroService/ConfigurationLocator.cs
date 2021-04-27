using Microsoft.Extensions.Configuration;
using System;

namespace Connecterra.StateTracking.Persistent.Connection
{
    public class ConfigurationLocator : IConfigurationLocator
    {
        public string EndpointUrl { get; set; }
        public string AuthorizationKey { get; set; }
        public string DatabaseId { get; set; }
        public string EventContainerId { get; set; }
        public string LeaseContainerId { get; set; }
        public string ViewContainerId { get; set; }
        public string SnapshotContainerId { get; set; }

        public ConfigurationLocator(IConfiguration configuration)
        {
            EndpointUrl = configuration["Database:EndpointUrl"];
            AuthorizationKey = configuration["Database:AuthorizationKey"];
            DatabaseId = configuration["Database:DatabaseId"];
            EventContainerId = configuration["Database:EventContainerId"];
            LeaseContainerId = configuration["Database:LeaseContainerId"];
            ViewContainerId = configuration["Database:ViewContainerId"];
            SnapshotContainerId = configuration["Database:SnapshotContainerId"];
        }
    }
}