using Eshop.Infrastructure.EventBus;
using Eshop.Infrastructure.Mongo;
using Eshop.Infrastructure.Mongo.Interface;
using Eshop.Infrastructure.Serilog;
using Eshop.Product.Query.Api.Handlers;
using Eshop.Product.Query.Api.Middlewares;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;

namespace Eshop.Product.Query.Api.DependencyInjections
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Eshop.Product.Query.Api", Version = "v1" });
            });

            services.AddMongoDb(configuration);
            services.AddScoped<GetProductByIdHandler>();

            // Serilog configuration
            var logger = LoggerConfig.Configure(configuration);
            services.AddSingleton(x => logger);

            // establish connection with rabbitmq 
            services.AddMassTransit(x =>
            {
                x.AddConsumer<GetProductByIdHandler>();
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var rabbitMq = new RabbitMqOption();
                    configuration.GetSection("RabbitMq").Bind(rabbitMq);
                    cfg.Host(new Uri(rabbitMq.ConnectionString), hostcfg =>
                    {
                        hostcfg.Username(rabbitMq.Username);
                        hostcfg.Password(rabbitMq.Password);
                    });
                    cfg.ConfigureEndpoints(provider);
                }));
            });
            return services;
        }

        public static IApplicationBuilder AddApplicationPipelines(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Eshop.Product.Query.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseMiddleware<RequestExceptionMiddleware>();

            app.UseAuthorization();

            var dbInitilizer = app.ApplicationServices.GetService<IDatabaseInitializer>();
            dbInitilizer.InitializeAsync();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
            app.ApplicationServices.GetService<IBusControl>().Start();

            return app;
        }
    }
}
