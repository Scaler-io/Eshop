using System.Threading.Tasks;
using Eshop.Infrastructure.Commands.User;
using Eshop.Infrastructure.Events.User;
using Eshop.Shared.Common;

namespace Eshop.User.DataAccess.Services
{
    public interface IUserService
    {
        Task<Result<UserCreated>> UpsertUser(CreateUser user);
        Task<Result<UserCreated>> GetUser(string id);
        Task<Result<bool>> DeleteUser(string id);
    }
}