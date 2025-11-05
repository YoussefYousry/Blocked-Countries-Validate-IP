namespace BlockedCountries.API.Controllers.Ip
{
    [ApiController]
    [Route("api/ip")]
    [ApiExplorerSettings(GroupName = "BlockedCountries")]
    public class IpController : ControllerBase
    {
        private readonly IIpLookupService _ipService;
        private readonly IResponseModel _response;

        public IpController(IIpLookupService ipService, IResponseModel response)
        {
            _ipService = ipService;
            _response = response;
        }

        /// <summary>
        /// Lookup country info by IP address. If ipAddress omitted, uses caller IP.
        /// </summary>
        /// <param name="ipAddress">IPv4/IPv6 address. Optional.</param>
        [HttpGet("lookup")]
        public async Task<IResponseModel> Lookup([FromQuery] string? ipAddress)
        {
            if (!string.IsNullOrWhiteSpace(ipAddress) && !System.Net.IPAddress.TryParse(ipAddress, out _))
                return _response.Fail("Invalid IP address format", (int)StatusCodesEnum.BadRequest);

            return await _ipService.LookupAsync(ipAddress);
        }

        /// <summary>
        /// Check if an IP (or caller IP) is blocked by country. Logs only blocked attempts.
        /// </summary>
        [HttpGet("check-block")]
        public async Task<IResponseModel> CheckBlock([FromQuery] string? ipAddress = null)
        {
            return await _ipService.CheckBlockAsync(ipAddress);
        }

    }
}


