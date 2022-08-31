using Eshop.Infrastructure.Mappers;
using Eshop.Product.DataAccess.Repositories;
using Eshop.Product.DataAccess.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Eshop.Product.DataAccess.DependencyInjections
{
    public static class AddDataLayerExtensions
    {
        public static IServiceCollection AddDataLayerServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ProductMappingProfile).Assembly);

            services.AddScoped<IProductService, ProductService>()
                    .AddScoped<IProductRepository, ProductRepository>();

            return services;
        }
    }
}
