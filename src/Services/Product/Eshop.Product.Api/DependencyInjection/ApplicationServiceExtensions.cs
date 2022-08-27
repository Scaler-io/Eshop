using Eshop.Infrastructure.EventBus;
using Eshop.Infrastructure.Mongo;
using Eshop.Infrastructure.Mongo.Interface;
using Eshop.Infrastructure.Serilog;
using Eshop.Product.Api.Handlers;
using Eshop.Product.Api.Middlewares;
using Eshop.Product.Api.Repositories;
using Eshop.Product.Api.Services;
using Eshop.Shared.Common;
using Eshop.Shared.Constants;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Eshop.Product.Api.DependencyInjection
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Eshop.Product.Api", Version = "v1" });
            });

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = ApiVersion.Default;
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
            })
            .AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            })
            .AddMvc()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = HandleFrameorkValidationFailure();
            });

            // Mongodb configuration for product database
            services.AddMongoDb(configuration);

            services.AddScoped<IProductService, ProductService>()
                    .AddScoped<IProductRepository, ProductRepository>()
                    .AddScoped<CreateOrUpdateProductHandler>();

            // Serilog configuration
            var logger = LoggerConfig.Configure(configuration);
            services.AddSingleton(x => logger);

            // event bus connection
            var rabbitMqOption = new RabbitMqOption();
            configuration.GetSection("RabbitMq").Bind(rabbitMqOption);

            services.AddMassTransit(x =>
            {
                x.AddConsumer<CreateOrUpdateProductHandler>();                  // Tells about the consumer
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
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
                }));
            });

            return services;
        }

        public static IApplicationBuilder AddConfigurationPiepline(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Eshop.Product.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseMiddleware<RequestExceptionMiddleware>();

            app.UseAuthorization();

            app.ApplicationServices.GetService<IDatabaseInitializer>().InitializeAsync();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var busControl = app.ApplicationServices.GetService<IBusControl>();
            busControl.Start();

            return app;
        }

        private static Func<ActionContext, IActionResult> HandleFrameorkValidationFailure()
        {
            return actionContext =>
            {
                var errors = actionContext.ModelState
                            .Where(e => e.Value.Errors.Count > 0).ToList();

                var validationError = new ApiValidationResponse()
                {
                    Errors = new List<FieldLevelError>()
                };

                foreach (var error in errors)
                {
                    var fieldLevelError = new FieldLevelError
                    {
                        Code = "Invalid",
                        Field = error.Key,
                        Message = error.Value.Errors?.First().ErrorMessage
                    };

                    validationError.Errors.Add(fieldLevelError);
                }

                return new UnprocessableEntityObjectResult(validationError);
            };
        }
    }
}
