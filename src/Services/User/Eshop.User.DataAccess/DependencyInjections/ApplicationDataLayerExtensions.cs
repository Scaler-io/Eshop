using Eshop.Infrastructure.Mappers;
using Eshop.User.DataAccess.Repositories;
using Eshop.User.DataAccess.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Eshop.User.DataAccess.DependencyInjections
{
    public static class ApplicationDataLayerExtensions
    {
        public static IServiceCollection AddDAtaLayerServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(UserMappingProfile).Assembly);
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            return services;
        }
    }
}
