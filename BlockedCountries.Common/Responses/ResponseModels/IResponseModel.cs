using BlockedCountries.Common.Enums;

namespace BlockedCountries.Common.Responses.ResponseModels
{
    public interface IResponseModel
    {
        ResponseModel Response(
            int statusCode,
            bool isError,
            string message,
            dynamic? data);

        ResponseModel Success(
            dynamic? data,
            string message,
            int statusCode = (int)StatusCodesEnum.Ok,
            bool isError = false);

        ResponseModel Fail(
            string message,
            int statusCode = (int)StatusCodesEnum.BadRequest,
            dynamic? data = null,
            bool isError = true);
    }
}
