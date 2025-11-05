namespace BlockedCountries.Common.Dtos.Models.BlockedCountries
{
    public class TemporalBlockRequestDto
    {
        public string CountryCode { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
    }
}
