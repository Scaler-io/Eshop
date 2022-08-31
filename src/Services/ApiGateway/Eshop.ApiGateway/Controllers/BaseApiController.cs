using Eshop.Shared.Common;
using Eshop.Shared.Constants;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Eshop.ApiGateway.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        public ILogger Logger { get; set; }
        public BaseApiController(ILogger logger)
        {
            Logger = logger;
        }

        public IActionResult OkOrFail<T>(Result<T> result)
        {
            if (result == null) return NotFound(new ApiResponse(ErrorCodes.NotFound, ErrorMessages.NotFound));
            if (result.IsSuccess && result.Value == null) return NotFound(new ApiResponse(ErrorCodes.NotFound));
            if (result.IsSuccess && result.Value != null) return Ok(result.Value);

            switch (result.ErrorCode)
            {
                case ErrorCodes.NotFound:
                    return NotFound(new ApiResponse(ErrorCodes.NotFound, result.ErrorMessage ?? ErrorMessages.NotFound));
                case ErrorCodes.Unauthorized:
                    return Unauthorized(new ApiResponse(ErrorCodes.Unauthorized, result.ErrorMessage ?? ErrorMessages.Unauthorized));
                case ErrorCodes.Operationfailed:
                    return BadRequest(new ApiResponse(ErrorCodes.Operationfailed, result.ErrorMessage ?? ErrorMessages.Operationfailed));
                default:
                    return BadRequest(new ApiResponse(ErrorCodes.BadRequest, result.ErrorMessage ?? ErrorMessages.BadRequest));
            }
        }
    }
}
