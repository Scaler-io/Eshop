using Eshop.Shared.Constants;

namespace Eshop.Shared.Common
{
    public class ApiResponse
    {
        public string Code { get; set; }
        public string Message { get; set; }

        public ApiResponse(string code, string message = null)
        {
            Code = code;
            Message = message;
        }

        protected virtual string GetDefaultMessage(string statusCode)
        {
            return statusCode switch
            {
                ErrorCodes.BadRequest => ErrorMessages.BadRequest,
                ErrorCodes.Unauthorized => ErrorMessages.Unauthorized,
                ErrorCodes.NotFound => ErrorMessages.NotFound,
                ErrorCodes.Operationfailed => ErrorMessages.Operationfailed,
                ErrorCodes.InternalServerError => ErrorMessages.InternalServerError,
                _ => null
            };
        }
    }
}
