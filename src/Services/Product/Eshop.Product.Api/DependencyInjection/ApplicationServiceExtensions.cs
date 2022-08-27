using Eshop.Infrastructure.Mongo;
using Eshop.Infrastructure.Mongo.Interface;
using Eshop.Infrastructure.Serilog;
using Eshop.Product.Api.Middlewares;
using Eshop.Product.Api.Repositories;
using Eshop.Product.Api.Services;
using Eshop.Shared.Common;
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

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductRepository, ProductRepository>();

            // Serilog configuration
            var logger = LoggerConfig.Configure(configuration);
            services.AddSingleton(x => logger);
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
