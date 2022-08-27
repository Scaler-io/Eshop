using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Eshop.Infrastructure.EventBus
{
    public static class Extension
    {
        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMq = new RabbitMqOption();
            configuration.GetSection("RabbitMq").Bind(rabbitMq);

            // establish connection with rabbitmq
            services.AddMassTransit(x =>
            {
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Host(new Uri(rabbitMq.ConnectionString), hostcfg =>
                    {
                        hostcfg.Username(rabbitMq.Username);
                        hostcfg.Password(rabbitMq.Password);
                    });
                }));
            });

            return services;
        }
    }
}
