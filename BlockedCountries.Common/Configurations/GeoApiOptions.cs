namespace BlockedCountries.Common.Configurations
{
    public class GeoApiOptions
    {
        public string BaseUrl { get; set; } = "https://ipapi.co/";
        public string? ApiKey { get; set; }
        public string? ApiKeyQueryName { get; set; }
    }
}


