using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace BlockedCountries.Infrastructure.Logging.Configurations
{
    public static class LoggingConfiguration
    {
        public static void AddSerilogLogging(this IHostBuilder hostBuilder)
        {
            hostBuilder.UseSerilog((context, services, config) =>
            {
                config.MinimumLevel.Information()
                      .Enrich.FromLogContext()
                      .WriteTo.Console()
                      .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                      .MinimumLevel.Override("System", LogEventLevel.Warning);
            });
        }
    }

}
