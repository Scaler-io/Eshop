using Eshop.Shared.Constants;
using System.Collections.Generic;

namespace Eshop.Shared.Common
{
    public class ApiValidationResponse : ApiResponse
    {
        public List<FieldLevelError> Errors { get; set; }
        public ApiValidationResponse()
            : base(ErrorCodes.UnprocessableEntity)
        {
            Message = GetDefaultMessage(ErrorCodes.UnprocessableEntity);
        }

        protected override string GetDefaultMessage(string statusCode)
        {
            return statusCode switch
            {
                ErrorCodes.UnprocessableEntity => "Inputs are invalid",
                _ => null
            };
        }
    }

    public class FieldLevelError
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Field { get; set; }
    }
}

