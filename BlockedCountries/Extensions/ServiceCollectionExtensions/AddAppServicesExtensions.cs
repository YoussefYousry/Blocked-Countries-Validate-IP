
namespace BlockedCountries.API.Extensions.ServiceCollectionExtensions
{
    public static class AddAppServicesExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddSingleton<IBlockedCountryRepository, InMemoryBlockedCountryRepository>();
            services.AddSingleton<IAttemptLogRepository, InMemoryAttemptLogRepository>();
            services.AddSingleton<ILogRepository, InMemoryLogRepository>();
            services.AddScoped<IResponseModel, ResponseModel>();

            services.AddScoped<ICountryBlockService,CountryBlockService>();
            services.AddScoped<IIpLookupService,IpLookupService>();
            services.AddScoped<ILogsService, LogsService>();


            services.AddOptions<GeoApiOptions>()
                .BindConfiguration("GeoApi")
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddHttpClient("geo", (sp, c) =>
            {
                var opts = sp.GetRequiredService<IOptions<GeoApiOptions>>().Value;
                c.BaseAddress = new Uri(opts.BaseUrl);
            });

            services.AddHostedService<TemporalBlockCleanupService>();

            return services;
        }
    }
}