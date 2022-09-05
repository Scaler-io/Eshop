using AutoMapper;
using Eshop.Infrastructure.Commands.User;
using Eshop.Infrastructure.Events.User;

namespace Eshop.Infrastructure.Mappers
{
    public class UserMappingProfile: Profile
    {
        public UserMappingProfile()
        {
            CreateMap<CreateUser, UserCreated>();
        }
    }
}
