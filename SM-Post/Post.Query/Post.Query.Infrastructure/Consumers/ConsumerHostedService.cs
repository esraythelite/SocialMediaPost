using CQRS.Core.Consumers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query_Infrastructure.Consumers
{
    public class ConsumerHostedService : IHostedService
    {
        private readonly ILogger<ConsumerHostedService> logger;
        
        private readonly IServiceProvider serviceProvider;

        private readonly IConfiguration config;

        public ConsumerHostedService( ILogger<ConsumerHostedService> logger, IServiceProvider serviceProvider, IConfiguration config )
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.config = config;
        }
        public Task StartAsync( CancellationToken cancellationToken )
        {
            logger.LogInformation("Event consumer service running");
            
            using(IServiceScope scope = serviceProvider.CreateScope())
            {
                var eventConsmer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();
                var topic = config.GetSection("KafkaConfiguration:KAFKA_TOPIC").Value; ;

                Task.Run(() => eventConsmer.Consume(topic), cancellationToken);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync( CancellationToken cancellationToken )
        {
            throw new NotImplementedException();
        }
    }
}
