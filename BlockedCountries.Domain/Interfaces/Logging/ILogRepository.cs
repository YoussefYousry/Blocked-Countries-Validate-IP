using BlockedCountries.Domain.Models.Logging;

namespace BlockedCountries.Domain.Interfaces.Logging
{
    public interface ILogRepository
    {
        Task AddAsync(LogEntry log, CancellationToken cancellationToken = default);
        Task<IEnumerable<LogEntry>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
        int Count { get; }
    }
}
