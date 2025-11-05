using BlockedCountries.Domain.Interfaces.BlockedCountries;
using BlockedCountries.Domain.Models.BlockedCountries;
using System.Collections.Concurrent;

namespace BlockedCountries.Infrastructure.Persistence
{
    public class InMemoryAttemptLogRepository : IAttemptLogRepository
    {
        private readonly ConcurrentBag<BlockedAttempt> _attempts = new();

        public int Count => _attempts.Count;

        public Task AddAsync(BlockedAttempt attempt , CancellationToken cancellationToken)
        {
            _attempts.Add(attempt);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<BlockedAttempt>> GetAllAsync(int page, int pageSize , CancellationToken cancellationToken)
        {
            var list = _attempts.OrderByDescending(x => x.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
            return Task.FromResult(list);
        }
    }
}
