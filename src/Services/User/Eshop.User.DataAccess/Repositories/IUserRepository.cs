using Eshop.Infrastructure.Commands.User;
using Eshop.Infrastructure.Events.User;
using System.Threading.Tasks;

namespace Eshop.User.DataAccess.Repositories
{
    public interface IUserRepository
    {
        Task<UserCreated> UpsertUser(CreateUser user);
        Task<UserCreated> GetUser(string userId);
        Task<bool> DeleteUser(string userId);
    }
}
