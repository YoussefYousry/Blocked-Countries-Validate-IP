namespace BlockedCountries.Domain.Models.BlockedCountries
{
    public class BlockedCountry
    {
        public string CountryCode { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public DateTime? ExpiresAt { get; set; } 
    }
}
