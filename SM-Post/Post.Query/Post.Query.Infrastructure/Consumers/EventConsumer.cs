using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using Post.Query.Infrastructure.Converters;
using Post.Query.Infrastructure.Handlers;
using System.Text.Json;

namespace Post.Query.Infrastructure.Consumers
{
    public class EventConsumer : IEventConsumer
    {
        private readonly ConsumerConfig config;
        private readonly IEventHandler eventHandler;

        public EventConsumer(IOptions<ConsumerConfig> config, IEventHandler eventHandler)
        {
            this.config = config.Value;
            this.eventHandler = eventHandler;
        }

        public void Consume( string topic )
        {
            using var consumer = new ConsumerBuilder<string, string>(config)
                .SetKeyDeserializer(Deserializers.Utf8)
                .SetValueDeserializer(Deserializers.Utf8)
                .Build();

            consumer.Subscribe(topic);

            while (true)
            {
                var consumeResult = consumer.Consume();

                if (consumeResult?.Message == null) continue;

                var options = new JsonSerializerOptions
                {
                    Converters = { new EventJsonConverter() }
                };

                var evnt = JsonSerializer.Deserialize<BaseEvent>(consumeResult.Message.Value, options);
                var handlerMethod = eventHandler.GetType().GetMethod("On", new Type[] { evnt.GetType() });

                if (handlerMethod == null)
                {
                    throw new ArgumentNullException(nameof(handlerMethod), "Could not find event handler method!");
                }

                handlerMethod.Invoke(eventHandler, new object[] { evnt });
                consumer.Commit(consumeResult);
            }
        }
    }
}
