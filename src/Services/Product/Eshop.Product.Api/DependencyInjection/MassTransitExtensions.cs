using Eshop.Infrastructure.EventBus;
using Eshop.Product.Api.Handlers;
using Eshop.Shared.Constants;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Eshop.Product.Api.DependencyInjection
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection AddMassTransitConfigs(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<CreateOrUpdateProductHandler>();                  // Tells about the consumer
                x.AddConsumer<DeleteProductHandler>();
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var rabbitMqOption = new RabbitMqOption();
                    configuration.GetSection("RabbitMq").Bind(rabbitMqOption);

                    cfg.Host(new Uri(rabbitMqOption.ConnectionString), hostcfg =>
                    {
                        hostcfg.Username(rabbitMqOption.Username);
                        hostcfg.Password(rabbitMqOption.Password);
                    });

                    cfg.ReceiveEndpoint(EventBusQueueNames.ProductQueueNames.CreateOrUpdate, ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(retryConfig => retryConfig.Interval(2, 100));
                        ep.ConfigureConsumer<CreateOrUpdateProductHandler>(provider);
                    });

                    cfg.ReceiveEndpoint(EventBusQueueNames.ProductQueueNames.Delete, ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(retryConfig => retryConfig.Interval(2, 100));
                        ep.ConfigureConsumer<DeleteProductHandler>(provider);
                    });
                }));
            });
            return services;
        }
    }
}
