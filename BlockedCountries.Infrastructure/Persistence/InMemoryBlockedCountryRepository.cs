using BlockedCountries.Domain.Interfaces.BlockedCountries;
using BlockedCountries.Domain.Models.BlockedCountries;
using System.Collections.Concurrent;

namespace BlockedCountries.Infrastructure.Persistence
{
    public class InMemoryBlockedCountryRepository : IBlockedCountryRepository
    {
        private readonly ConcurrentDictionary<string, BlockedCountry> _countries = new(StringComparer.OrdinalIgnoreCase);

        public Task<bool> AddAsync(BlockedCountry country, CancellationToken cancellationToken = default)
        {
            if (country is null || string.IsNullOrWhiteSpace(country.CountryCode))
                return Task.FromResult(false);

            var key = country.CountryCode.ToUpperInvariant();
            var success = _countries.TryAdd(key, country);
            return Task.FromResult(success);
        }

        public Task<bool> RemoveAsync(string countryCode, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                return Task.FromResult(false);

            var key = countryCode.ToUpperInvariant();
            var success = _countries.TryRemove(key, out _);
            return Task.FromResult(success);
        }

        public Task<IEnumerable<BlockedCountry>> GetAllAsync(string? query = null, CancellationToken cancellationToken = default)
        {
            var list = _countries.Values.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.ToUpperInvariant();
                list = list.Where(x =>
                    x.CountryCode.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrWhiteSpace(x.CountryName) && x.CountryName.Contains(q, StringComparison.OrdinalIgnoreCase))
                );
            }

            return Task.FromResult(list);
        }

        public Task<BlockedCountry?> GetByCodeAsync(string countryCode, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                return Task.FromResult<BlockedCountry?>(null);

            var key = countryCode.ToUpperInvariant();
            _countries.TryGetValue(key, out var result);
            return Task.FromResult(result);
        }
    }
}
