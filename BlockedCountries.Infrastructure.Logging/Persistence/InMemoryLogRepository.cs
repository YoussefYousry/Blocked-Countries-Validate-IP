using BlockedCountries.Domain.Interfaces.Logging;
using BlockedCountries.Domain.Models.Logging;
using System.Collections.Concurrent;

namespace BlockedCountries.Infrastructure.Logging.Persistence
{
    public class InMemoryLogRepository : ILogRepository
    {
        private readonly ConcurrentBag<LogEntry> _logs = new();

        public int Count => _logs.Count;

        public Task AddAsync(LogEntry log, CancellationToken cancellationToken = default)
        {
            _logs.Add(log);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<LogEntry>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var logs = _logs.OrderByDescending(l => l.Timestamp)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

            return Task.FromResult<IEnumerable<LogEntry>>(logs);
        }
    }
}
