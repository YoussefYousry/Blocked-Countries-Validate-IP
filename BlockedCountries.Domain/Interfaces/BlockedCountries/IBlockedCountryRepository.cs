using BlockedCountries.Domain.Models.BlockedCountries;

namespace BlockedCountries.Domain.Interfaces.BlockedCountries
{
    public interface IBlockedCountryRepository
    {
        Task<bool> AddAsync(BlockedCountry country,CancellationToken cancellationToken = default);
        Task<bool> RemoveAsync(string countryCode , CancellationToken cancellationToken = default);
        Task<IEnumerable<BlockedCountry>> GetAllAsync(string? query = null, CancellationToken cancellationToken = default);
        Task<BlockedCountry?> GetByCodeAsync(string countryCode, CancellationToken cancellationToken);
    }
}
