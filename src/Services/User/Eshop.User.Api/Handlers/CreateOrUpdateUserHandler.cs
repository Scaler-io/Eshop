using System.Threading.Tasks;
using Eshop.Infrastructure.Commands.User;
using Eshop.Shared.Extensions;
using Eshop.User.DataAccess.Services;
using MassTransit;
using Serilog;

namespace Eshop.User.Api.Handlers
{
    public class CreateOrUpdateUserHandler : IConsumer<CreateUser>
    {
        private readonly ILogger _logger;
        private readonly IUserService _userService;

        public CreateOrUpdateUserHandler(ILogger logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public async Task Consume(ConsumeContext<CreateUser> context)
        {
            _logger.Here().MethodEnterd();
            await _userService.UpsertUser(context.Message);
            _logger.Here().MethodExited();
        }
    }
}