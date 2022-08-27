using Eshop.Shared.Constants;

namespace Eshop.Shared.Common
{
    public class ApiExceptionResponse: ApiResponse
    {
        public string StackTrace { get; set; }
        public ApiExceptionResponse(string errorMessage = null, string stackTrace = null)
            : base(ErrorCodes.InternalServerError, errorMessage)
        {
            StackTrace = stackTrace;
        }
    }
}
