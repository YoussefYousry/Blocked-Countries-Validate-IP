namespace BlockedCountries.API.Controllers.Countries
{
    [ApiController]
    [Route("api/countries")]
    [ApiExplorerSettings(GroupName = "BlockedCountries")]
    public class CountriesController : ControllerBase
    {
        private readonly ICountryBlockService _service;
        private readonly IResponseModel _response;
        public CountriesController(ICountryBlockService service, IResponseModel response)
        {
            _service = service;
            _response = response;
        }

        /// <summary>
        /// Add a blocked country permanently.
        /// </summary>
        /// <param name="request">Country code and optional name.</param>
        /// <response code="200">Country added.</response>
        /// <response code="409">Country already blocked.</response>
        [HttpPost("block")]
        public async Task<IResponseModel> Block([FromBody] BlockCountryRequest request)
        {
            return await _service.AddAsync(request.CountryCode, request.CountryName);
        }

        /// <summary>
        /// Remove a country from blocked list.
        /// </summary>
        /// <param name="code">Country alpha-2 code.</param>
        /// <response code="204">Removed.</response>
        /// <response code="404">Not found.</response>
        [HttpDelete("block/{code}")]
        public async Task<IResponseModel> Unblock(string code)
        {
            return await _service.RemoveAsync(code);
        }

        /// <summary>
        /// Get all blocked countries with pagination and search.
        /// </summary>
        /// <param name="SearchValue">Search by code or name.</param>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        [HttpGet("blocked")]
        public async Task<IResponseModel> GetAll([FromQuery] string? SearchValue = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            return await _service.GetAllAsync(SearchValue, page, pageSize);
        }

        /// <summary>
        /// Temporarily block a country for a duration.
        /// </summary>
        /// <param name="request">Country code and duration in minutes (1-1440).</param>
        /// <response code="200">Country temporarily blocked.</response>
        /// <response code="400">Invalid request.</response>
        /// <response code="409">Already temporarily blocked.</response>
        [HttpPost("temporal-block")]
        public async Task<IResponseModel> TemporalBlock([FromBody] TemporalBlockRequestDto request)
        {
            if (request.DurationMinutes < 1 || request.DurationMinutes > 1440)
                return _response.Fail("durationMinutes must be between 1 and 1440", (int)StatusCodesEnum.BadRequest);

            return await _service.AddTemporalAsync(request.CountryCode, null, request.DurationMinutes);
        }
    }
}
