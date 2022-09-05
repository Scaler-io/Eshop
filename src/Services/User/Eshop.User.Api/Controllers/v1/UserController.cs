using System.Net;
using System.Threading.Tasks;
using Eshop.Infrastructure.Commands.User;
using Eshop.Infrastructure.Events.User;
using Eshop.Shared.Common;
using Eshop.Shared.Extensions;
using Eshop.User.DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Eshop.User.Api.Controllers.v1
{
    [ApiVersion("1")]
    public class UserController: BaseApiController
    {
        private readonly IUserService _userService;
        public UserController(ILogger logger, IUserService userService)
        : base(logger)
        {
            _userService = userService;
        }

        [HttpGet("{id}", Name = "GetUser")]
        [ProducesResponseType(typeof(UserCreated), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiValidationResponse), (int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType(typeof(ApiExceptionResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetUser([FromRoute] string id){
            Logger.Here().MethodEnterd();
            var response = await _userService.GetUser(id);
            Logger.Here().MethodExited();
            return OkOrFail(response);
        }

        [HttpPost("upsert", Name = "UpsertUser")]
        [ProducesResponseType(typeof(UserCreated), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiValidationResponse), (int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType(typeof(ApiExceptionResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateOrUpdateUser([FromBody] CreateUser user){
            Logger.Here().MethodEnterd();
            var response = await _userService.UpsertUser(user);
            Logger.Here().MethodExited();
            return OkOrFail(response);
        }

        [HttpDelete("{id}/delete", Name = "DeleteUser")]
        [ProducesResponseType(typeof(UserCreated), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiValidationResponse), (int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType(typeof(ApiExceptionResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteUser([FromRoute] string id){
            Logger.Here().MethodEnterd();
            var response = await _userService.DeleteUser(id);
            Logger.Here().MethodExited();
            return OkOrFail(response);
        }

    }
}