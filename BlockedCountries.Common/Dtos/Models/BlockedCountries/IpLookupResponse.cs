namespace BlockedCountries.Common.Dtos.Models.BlockedCountries
{
    public class IpLookupResponseDto
    {
        public string? CountryCode { get; set; }
        public string? CountryName { get; set; }
        public string? Isp { get; set; }
    }
}
