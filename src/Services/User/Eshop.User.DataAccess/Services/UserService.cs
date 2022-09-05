using System.Threading.Tasks;
using Eshop.Infrastructure.Commands.User;
using Eshop.Infrastructure.Events.User;
using Eshop.Shared.Common;
using Eshop.Shared.Constants;
using Eshop.Shared.Extensions;
using Eshop.User.DataAccess.Repositories;
using Serilog;

namespace Eshop.User.DataAccess.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;

        public UserService(ILogger logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task<Result<UserCreated>> GetUser(string id)
        {
            _logger.Here().MethodEnterd();
            var result = await _userRepository.GetUser(id);

            if(result == null){
                _logger.Here().Error("{@ErrorCode} No user was found with {@id}", ErrorCodes.NotFound, id);
                return Result<UserCreated>.Fail(ErrorCodes.NotFound);
            }

            _logger.Here().Error("User details found {@user}", result);
            _logger.Here().MethodExited();
            return Result<UserCreated>.Success(result);
        }

        public async Task<Result<UserCreated>> UpsertUser(CreateUser user)
        {
            _logger.Here().MethodEnterd();
            var result = await _userRepository.UpsertUser(user);
            
            _logger.Here().Error("User created successfully {@user}", result);
            _logger.Here().MethodExited();
            return Result<UserCreated>.Success(result);
        }

        public async Task<Result<bool>> DeleteUser(string id)
        {
             _logger.Here().MethodEnterd();
            var result = await _userRepository.DeleteUser(id);
            
            if(!result){
                _logger.Here().Error("{@ErrorCode} Failed to delete user with {@id}", ErrorCodes.Operationfailed, id);
                return Result<bool>.Fail(ErrorCodes.Operationfailed, "Failed to delete user");
            }
            _logger.Here().Error("User created successfully {@user}", result);
            _logger.Here().MethodExited();
            return Result<bool>.Success(result);
        }

    }
}