using BlockedCountries.Common.Responses.ResponseModels;

namespace BlockedCountries.Application.Services.BlockedCountries.CountryBlock
{
    public interface ICountryBlockService
    {

        Task<IResponseModel> AddAsync(string code, string? name);
        Task<IResponseModel> RemoveAsync(string code);
        Task<IResponseModel> GetAllAsync(string? search, int page, int pageSize);
        Task<bool> IsBlockedAsync(string code);
        Task<IResponseModel> AddTemporalAsync(string code, string? name, int durationMinutes);
        Task<int> RemoveExpiredAsync();
    }
}
