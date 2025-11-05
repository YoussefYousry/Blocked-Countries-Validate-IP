using BlockedCountries.Common.Enums;
using BlockedCountries.Common.Responses.PaginationModels;
using BlockedCountries.Common.Responses.ResponseModels;
using BlockedCountries.Domain.Interfaces.BlockedCountries;
using BlockedCountries.Domain.Models.BlockedCountries;

namespace BlockedCountries.Application.Services.BlockedCountries.CountryBlock
{
    public class CountryBlockService : ICountryBlockService
    {
        private readonly IBlockedCountryRepository _countryRepo;
        private readonly IResponseModel _response;

        public CountryBlockService(IBlockedCountryRepository countryRepo, IResponseModel response)
        {
            _countryRepo = countryRepo;
            _response = response;
        }

        private static bool IsValidCountryCode(string? code)
        {
            if (string.IsNullOrWhiteSpace(code) || code.Length != 2)
                return false;

            return code.All(char.IsLetter);
        }

        public async Task<IResponseModel> AddAsync(string code, string? name)
        {
            code = code.ToUpperInvariant();

            if (!IsValidCountryCode(code))
                return _response.Fail("Invalid country code", (int)StatusCodesEnum.BadRequest);

            var existing = await _countryRepo.GetByCodeAsync(code, default);
            if (existing != null)
                return _response.Fail("Country already blocked", (int)StatusCodesEnum.Conflict);

            var added = await _countryRepo.AddAsync(new BlockedCountry
            {
                CountryCode = code,
                CountryName = name ?? code
            }, default);

            return added
                ? _response.Success(null, "Country blocked")
                : _response.Fail("Failed to add country", (int)StatusCodesEnum.ServerError);
        }

        public async Task<IResponseModel> RemoveAsync(string code)
        {
            code = code.ToUpperInvariant();
            var removed = await _countryRepo.RemoveAsync(code, default);
            return removed
                ? _response.Success(null, "Country unblocked successfully")
                : _response.Fail("Country not found", (int)StatusCodesEnum.NotFound);
        }

        public async Task<IResponseModel> GetAllAsync(string? search, int page, int pageSize)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var all = await _countryRepo.GetAllAsync(search, default);
            var total = all.Count();

            var items = all
                .OrderBy(x => x.CountryCode)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var payload = new PagainationModel<IEnumerable<BlockedCountry>>
            {
                Data = items,
                TotalCount = total,
                PageNumber = page,
                PageSize = pageSize
            };

            return _response.Success(payload, "Blocked countries fetched successfully");
        }

        public async Task<bool> IsBlockedAsync(string code)
        {
            code = code.ToUpperInvariant();

            var block = await _countryRepo.GetByCodeAsync(code, default);

            if (block == null)
                return false;

            if (block.ExpiresAt.HasValue && block.ExpiresAt.Value <= DateTime.UtcNow)
                return false;

            return true;
        }


        public async Task<IResponseModel> AddTemporalAsync(string code, string? name, int durationMinutes)
        {
            code = code.ToUpperInvariant();

            if (!IsValidCountryCode(code))
                return _response.Fail("Invalid country code", (int)StatusCodesEnum.BadRequest);

            var exists = await _countryRepo.GetByCodeAsync(code, default);
            if (exists != null)
                return _response.Fail("Country already blocked", (int)StatusCodesEnum.Conflict);

            var expiresAt = DateTime.UtcNow.AddMinutes(durationMinutes);

            var added = await _countryRepo.AddAsync(new BlockedCountry
            {
                CountryCode = code,
                CountryName = name ?? code,
                ExpiresAt = expiresAt
            }, default);

            return added
                ? _response.Success(new { ExpiresAt = expiresAt }, "Country temporarily blocked")
                : _response.Fail("Failed to add temporal block", (int)StatusCodesEnum.ServerError);
        }

        public async Task<int> RemoveExpiredAsync()
        {
            var all = await _countryRepo.GetAllAsync(null, default);
            var now = DateTime.UtcNow;

            var expired = all
                .Where(x => x.ExpiresAt.HasValue && x.ExpiresAt.Value <= now)
                .ToList();

            if (!expired.Any())
                return 0;

            foreach (var country in expired)
                await _countryRepo.RemoveAsync(country.CountryCode, default);

            return expired.Count;
        }


    }
}
