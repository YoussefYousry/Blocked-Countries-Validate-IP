using BlockedCountries.Common.Enums;
using BlockedCountries.Common.Responses.ResponseModels;

namespace BlockedCountries.Common.Responses
{
    public class ResponseModel : IResponseModel
    {
        public int StatusCode { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsError { get; set; }
        public string Message { get; set; } = string.Empty;
        public dynamic? Result { get; set; }

        public ResponseModel Response(
            int statusCode,
            bool isError,
            string message,
            dynamic? data)
        {
            return new ResponseModel
            {
                Timestamp = DateTime.UtcNow,
                StatusCode = statusCode,
                IsError = isError,
                Message = message,
                Result = data
            };
        }

        public ResponseModel Success(
            dynamic? data,
            string message,
            int statusCode = (int)StatusCodesEnum.Ok,
            bool isError = false)
            => Response(statusCode, isError, message, data);

        public ResponseModel Fail(
            string message,
            int statusCode = (int)StatusCodesEnum.BadRequest,
            dynamic? data = null,
            bool isError = true)
            => Response(statusCode, isError, message, data);
    }
}
