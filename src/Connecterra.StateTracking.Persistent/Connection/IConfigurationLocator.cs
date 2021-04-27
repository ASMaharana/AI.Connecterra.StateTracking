using System;

namespace Connecterra.StateTracking.Persistent.Connection
{
    public interface IConfigurationLocator
    {
        string EndpointUrl { get; set; }
        string AuthorizationKey { get; set; }
        string DatabaseId { get; set; }
        string EventContainerId { get; set; }
        string LeaseContainerId { get; set; }
        string ViewContainerId { get; set; }
        string SnapshotContainerId { get; set; }
    }
}