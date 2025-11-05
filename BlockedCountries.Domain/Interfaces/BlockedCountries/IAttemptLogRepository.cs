using BlockedCountries.Domain.Models.BlockedCountries;

namespace BlockedCountries.Domain.Interfaces.BlockedCountries
{
    public interface IAttemptLogRepository
    {
        Task AddAsync(BlockedAttempt attempt , CancellationToken cancellationToken);
        Task<IEnumerable<BlockedAttempt>> GetAllAsync(int page, int pageSize , CancellationToken cancellationToken);
        int Count { get; }
    }
}
