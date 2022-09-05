using System.Net;
using System.Threading.Tasks;
using Eshop.ApiGateway.Services.User;
using Eshop.Infrastructure.Commands.User;
using Eshop.Shared.Common;
using Eshop.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Eshop.ApiGateway.Controllers
{
    [Route("user")]
    public class UserController: BaseApiController
    {
        private readonly IUserEvent _productEvent;

        public UserController(IUserEvent productEvent, ILogger logger)
            :base(logger)
        {
            _productEvent = productEvent;
        }

        [HttpPost("upsert", Name = "CreateOrUpdateUser")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.Accepted)]
        [ProducesResponseType(typeof(ApiExceptionResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateUser user)
        {
            Logger.Here().MethodEnterd();
            await _productEvent.PublishUpsertEventAsync(user);
            Logger.MethodExited();
            return Accepted("User created successfully");
        }

    }
}