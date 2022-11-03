using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using Post.Query_Infrastructure.Converters;
using Post.Query_Infrastructure.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Post.Query_Infrastructure.Consumers
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
                var consumerResult = consumer.Consume();

                if (consumerResult?.Message == null) continue;

                var options = new JsonSerializerOptions
                {
                    Converters = { new EventJsonConverter() }
                };

                var evnt = JsonSerializer.Deserialize<BaseEvent>(consumerResult.Message.Value, options);
                var handlerMethod = eventHandler.GetType().GetMethod("On", new Type[] { evnt.GetType() });

                if (handlerMethod == null)
                {
                    throw new ArgumentNullException(nameof(handlerMethod), "Could not find event handler method!");
                }

                handlerMethod.Invoke(eventHandler, new object[] { evnt });
                consumer.Commit(consumerResult);
            }
        }
    }
}
