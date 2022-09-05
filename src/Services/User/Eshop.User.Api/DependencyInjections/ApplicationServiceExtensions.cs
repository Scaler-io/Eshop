using Eshop.Infrastructure.Mongo;
using Eshop.Infrastructure.Mongo.Interface;
using Eshop.Infrastructure.Serilog;
using Eshop.Shared.Common;
using Eshop.User.Api.Middlewares;
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

namespace Eshop.User.Api.DependencyInjections
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Eshop.User.Api", Version = "v1" });
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

            // Serilog configuration
            var logger = LoggerConfig.Configure(configuration);
            services.AddSingleton(x => logger);

            return services;
        }
        
        public static IApplicationBuilder AddApplicationPipelines(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Eshop.User.Api v1"));
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
