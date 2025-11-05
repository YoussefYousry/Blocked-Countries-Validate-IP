namespace BlockedCountries.Domain.Models.BlockedCountries
{
    public class BlockedAttempt
    {
        public string IpAddress { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string CountryCode { get; set; } = string.Empty;
        public bool Blocked { get; set; }
        public string? UserAgent { get; set; }
    }
}
