namespace BlockedCountries.API.HostedServices
{
    public class TemporalBlockCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TemporalBlockCleanupService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

        public TemporalBlockCleanupService(IServiceProvider serviceProvider, ILogger<TemporalBlockCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TemporalBlockCleanupService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var countryBlockService = scope.ServiceProvider.GetRequiredService<ICountryBlockService>();
                    var removedCount = await countryBlockService.RemoveExpiredAsync();
                    _logger.LogInformation("Cleanup complete: {Count} expired blocks removed.", removedCount);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during cleanup process.");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }

}
