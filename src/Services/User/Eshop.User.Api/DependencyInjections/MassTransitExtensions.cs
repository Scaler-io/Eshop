using System;
using Eshop.Infrastructure.EventBus;
using Eshop.User.Api.Handlers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static Eshop.Shared.Constants.EventBusQueueNames;

namespace Eshop.User.Api.DependencyInjections
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection AddMassTransitConfigs(this IServiceCollection services, IConfiguration configuration){
            
            services.AddMassTransit(x => {
                x.AddConsumer<CreateOrUpdateUserHandler>();
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg => {
                     var rabbitMqOption = new RabbitMqOption();
                    configuration.GetSection("RabbitMq").Bind(rabbitMqOption);
                    
                    cfg.Host(new Uri(rabbitMqOption.ConnectionString), hostConfig => {
                        hostConfig.Username(rabbitMqOption.Username);
                        hostConfig.Password(rabbitMqOption.Password);
                    });
                    
                    cfg.ReceiveEndpoint(UserQueueNames.CreateOrUpdate, ep => {
                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(retryConfig => retryConfig.Interval(2, 100));
                        ep.ConfigureConsumer<CreateOrUpdateUserHandler>(provider);
                    });
                }));
            });
            
            return services;
        }
    }
}