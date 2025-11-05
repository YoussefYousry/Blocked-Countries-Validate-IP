using BlockedCountries.Common.Responses.ResponseModels;

namespace BlockedCountries.Application.Services.BlockedCountries.IpLookup
{
    public interface IIpLookupService
    {
        Task<IResponseModel> LookupAsync(string? ip);
        Task<IResponseModel> CheckBlockAsync(string? ipAddress);
    }
}
