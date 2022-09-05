using Eshop.ApiGateway.Services.Product;
using Eshop.ApiGateway.Services.User;
using Eshop.Infrastructure.EventBus;
using Eshop.Infrastructure.Serilog;
using Eshop.Product.Api.Middlewares;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Eshop.ApiGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Eshop.ApiGateway", Version = "v1" });
            });
            services.AddScoped<IProductEvent, ProductEvent>();
            services.AddScoped<IUserEvent, UserEvent>();
            services.AddRabbitMQ(Configuration);

            // Serilog configuration
            var logger = LoggerConfig.Configure(Configuration);
            services.AddSingleton(x => logger);
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Eshop.ApiGateway v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseMiddleware<RequestExceptionMiddleware>();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.ApplicationServices.GetService<IBusControl>().Start();
        }
    }
}
