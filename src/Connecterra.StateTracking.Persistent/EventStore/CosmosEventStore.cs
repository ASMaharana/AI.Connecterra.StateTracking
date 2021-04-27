using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Connecterra.StateTracking.Common.Core;
using Connecterra.StateTracking.Persistent.Connection;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Connecterra.StateTracking.Persistent.EventStore
{
    public interface IEventStore
    {
        Task<Result<EventStream>> LoadStreamAsync(string streamId);
        Task<Result<EventStream>> LoadStreamAsync(string streamId, int fromVersion);
        Task<Result> AppendToStreamAsync(string streamId, int expectedVersion, IEnumerable<IEvent> events);
    }
    public class CosmosEventStore : IEventStore
    {
        private readonly IDbClient _dbClient;
        private readonly IEventTypeResolver _eventTypeResolver;
        public CosmosEventStore(IDbClient dbClient, IEventTypeResolver eventTypeResolver)
        {
            _dbClient = dbClient;
            _eventTypeResolver = eventTypeResolver;
        }

        public async Task<Result<EventStream>> LoadStreamAsync(string streamId)
        {
            try
            {
                var container = _dbClient.GetEventContainer();

                var sqlQueryText = "SELECT * FROM events e"
                    + " WHERE e.stream.id = @streamId"
                    + " ORDER BY e.stream.version";

                var queryDefinition = new QueryDefinition(sqlQueryText)
                    .WithParameter("@streamId", streamId);

                var version = 0;
                var events = new List<IEvent>();

                var feedIterator = container.GetItemQueryIterator<EventWrapper>(queryDefinition);
                while (feedIterator.HasMoreResults)
                {
                    var response = await feedIterator.ReadNextAsync();
                    foreach (var eventWrapper in response)
                    {
                        version = eventWrapper.StreamInfo.Version;

                        events.Add(eventWrapper.GetEvent(_eventTypeResolver));
                    }
                }

                return Result.OK(new EventStream(streamId, version, events));
            }
            catch(Exception ex)
            {
                return Result.Error<EventStream>($"Fail to LoadStream for streamId:{streamId}");
            }
        }

        public async Task<Result<EventStream>> LoadStreamAsync(string streamId, int fromVersion)
        {
            try
            {
                var container = _dbClient.GetEventContainer();

                var sqlQueryText = "SELECT * FROM events e"
                    + " WHERE e.stream.id = @streamId AND e.stream.version >= @fromVersion"
                    + " ORDER BY e.stream.version";

                var queryDefinition = new QueryDefinition(sqlQueryText)
                    .WithParameter("@streamId", streamId)
                    .WithParameter("@fromVersion", fromVersion);

                var version = 0;
                var events = new List<IEvent>();

                var feedIterator = container.GetItemQueryIterator<EventWrapper>(queryDefinition);
                while (feedIterator.HasMoreResults)
                {
                    var response = await feedIterator.ReadNextAsync();
                    foreach (var eventWrapper in response)
                    {
                        version = eventWrapper.StreamInfo.Version;

                        events.Add(eventWrapper.GetEvent(_eventTypeResolver));
                    }
                }

                return Result.OK(new EventStream(streamId, version, events));
            }
            catch (Exception ex)
            {
                return Result.Error<EventStream>($"Fail to LoadStream for streamId:{streamId}, fromVersion: {fromVersion}");
            }
        }

        public async Task<Result> AppendToStreamAsync(string streamId, int expectedVersion, IEnumerable<IEvent> events)
        {
            try
            {
                var container = _dbClient.GetEventContainer();
                var partitionKey = new PartitionKey(streamId);
                var parameters = new dynamic[]
                {
                    streamId,
                    expectedVersion,
                    SerializeEvents(streamId, expectedVersion, events)
                };

                var response = await container.Scripts.ExecuteStoredProcedureAsync<bool>("sp_appendtostream", partitionKey, parameters);
                if (response.StatusCode == HttpStatusCode.OK)
                { 
                    return Result.OK(); 
                }
                throw new Exception();
            }
            catch (Exception ex)
            {
                return Result.Error($"Fail to AppendToStream for streamId:{streamId}, expectedVersion: {expectedVersion}");
            }
        }

        private static string SerializeEvents(string streamId, int expectedVersion, IEnumerable<IEvent> events)
        {
            var items = events.Select(e => new EventWrapper
            {
                Id = $"{streamId}:{++expectedVersion}:{e.GetType().Name}",
                StreamInfo = new StreamInfo
                {
                    Id = streamId,
                    Version = expectedVersion
                },
                EventType = e.GetType().Name,
                EventData = JObject.FromObject(e)
            });

            return JsonConvert.SerializeObject(items);
        }
    }
}