using Eshop.Product.Api.DependencyInjection;
using Eshop.Product.DataAccess.DependencyInjections;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Eshop.Product.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationServices(_configuration);
            services.AddDataLayerServices();

            // event bus connection
            services.AddMassTransitConfigs(_configuration); ;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.AddConfigurationPiepline(env);
        }
    }
}
