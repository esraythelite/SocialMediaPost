using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Microsoft.Extensions.Configuration;
using Post.Cmd.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Cmd.Infrastructure.Stores
{
    public class EventStore : IEventStore
    {
        private readonly IEventStoreRepository eventStoreRepository;
        private readonly IEventProducer eventProducer;

        public IConfiguration Config { get; }

        public EventStore(IEventStoreRepository eventStoreRepository, IEventProducer eventProducer)
        {
            this.eventStoreRepository = eventStoreRepository;
            this.eventProducer = eventProducer;
        }

        public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
        {
            var eventStream = await eventStoreRepository.FindByAggregateId(aggregateId);

            if (eventStream == null || eventStream.Any())
            {
                throw new AggregateNotFoundException("Incorrect post ID provided!");
            }

            return eventStream.OrderBy(x => x.Version).Select(x => x.EventData).ToList();
        }

        public async Task SaveEventAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
        {
            var eventStream = await eventStoreRepository.FindByAggregateId(aggregateId);

            if (expectedVersion != -1 && eventStream[^1].Version != expectedVersion)
            {
                throw new ConcurrencyException();
            }

            var version = expectedVersion;

            foreach (var evnt in events)
            {
                version++;
                evnt.Version = version;
                var eventType = evnt.GetType().Name;
                var eventModel = new EventModel
                {
                    TimeStamp = DateTime.Now,
                    AggregateIdentifier = aggregateId,
                    AggregateType = nameof(PostAggregate),
                    Version = version,
                    EventType = eventType,
                    EventData = evnt
                };

                await eventStoreRepository.SaveAsync(eventModel);

                var topic = Config.GetSection("KafkaConfiguration:KAFKA_TOPIC").Value;
                await eventProducer.ProduceAsync(topic, evnt);
            }
        }
    }
}
