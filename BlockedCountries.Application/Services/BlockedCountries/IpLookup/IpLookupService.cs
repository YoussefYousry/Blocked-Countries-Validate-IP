using BlockedCountries.Application.Services.BlockedCountries.CountryBlock;
using BlockedCountries.Application.Services.Logs;
using BlockedCountries.Common.Configurations;
using BlockedCountries.Common.Dtos.Models.BlockedCountries;
using BlockedCountries.Common.Enums;
using BlockedCountries.Common.Responses;
using BlockedCountries.Common.Responses.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Net;

namespace BlockedCountries.Application.Services.BlockedCountries.IpLookup
{
    public class IpLookupService : IIpLookupService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly GeoApiOptions _options;
        private readonly IResponseModel _response;
        private readonly ILogsService _logService;
        private readonly ICountryBlockService _countryService;


        public IpLookupService(IHttpContextAccessor accessor, 
            IHttpClientFactory factory,
            IOptions<GeoApiOptions> options, 
            IResponseModel response, 
            ILogsService logService, 
            ICountryBlockService countryService)
        {
            _httpContextAccessor = accessor;
            _httpClient = factory.CreateClient("geo");
            _logService = logService;
            _options = options.Value;
            _response = response;
            _countryService = countryService;
        }

        public async Task<IResponseModel> LookupAsync(string? ip)
        {
            ip ??= _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            if (!string.IsNullOrWhiteSpace(ip) && IPAddress.TryParse(ip, out var parsed) && IPAddress.IsLoopback(parsed))
            {
                return _response.Fail("Localhost/loopback IP cannot be resolved. Provide a public ipAddress.", (int)StatusCodesEnum.BadRequest);
            }
            string url;
            if ((_options.BaseUrl ?? string.Empty).Contains("ipgeolocation", StringComparison.OrdinalIgnoreCase))
            {
                var keyPart = !string.IsNullOrWhiteSpace(_options.ApiKey) && !string.IsNullOrWhiteSpace(_options.ApiKeyQueryName)
                    ? $"{_options.ApiKeyQueryName}={_options.ApiKey}"
                    : string.Empty;
                var ipPart = !string.IsNullOrWhiteSpace(ip) ? $"&ip={ip}" : string.Empty;
                url = $"ipgeo?{keyPart}{ipPart}";
            }
            else
            {
                url = $"{ip}/json/";
                if (!string.IsNullOrWhiteSpace(_options.ApiKey) && !string.IsNullOrWhiteSpace(_options.ApiKeyQueryName))
                {
                    url = $"{url}?{_options.ApiKeyQueryName}={_options.ApiKey}";
                }
            }
            try
            {
                var body = await _httpClient.GetStringAsync(url);
                var json = JObject.Parse(body);
                var code = json["country_code"]?.ToString() ?? json["country_code2"]?.ToString();
                var name = json["country_name"]?.ToString();
                var isp = json["org"]?.ToString() ?? json["isp"]?.ToString() ?? json["organization"]?.ToString();
                if (string.IsNullOrWhiteSpace(code) && string.IsNullOrWhiteSpace(name))
                {
                    return _response.Fail("Unable to resolve country for the provided IP. Try a public IP or different provider.", (int)StatusCodesEnum.BadRequest);
                }
                var dto = new IpLookupResponseDto { CountryCode = code, CountryName = name, Isp = isp };
                return _response.Success(dto, "Lookup successful");
            }
            catch (HttpRequestException ex)
            {
                return _response.Fail($"Geo API request failed: {ex.Message}", (int)StatusCodesEnum.ServerError);
            }
            catch (Exception ex)
            {
                return _response.Fail($"Geo API error: {ex.Message}", (int)StatusCodesEnum.ServerError);
            }
        }

        public async Task<IResponseModel> CheckBlockAsync(string? ipAddress)
        {
            var http = _httpContextAccessor.HttpContext!;
            if (!string.IsNullOrWhiteSpace(ipAddress) && !IPAddress.TryParse(ipAddress, out _))
                return _response.Fail("Invalid IP address format", (int)StatusCodesEnum.BadRequest);

            var callerIp = ipAddress ?? http.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
            var userAgent = http.Request.Headers["User-Agent"].ToString();

            var lookup = await LookupAsync(ipAddress) as ResponseModel;
            if (lookup == null)
                return _response.Fail("Lookup failed", (int)StatusCodesEnum.ServerError);
            if (lookup.IsError)
                return lookup;

            var result = lookup.Result as IpLookupResponseDto;
            var code = result?.CountryCode;
            var name = result?.CountryName;
            var isp = result?.Isp;

            var isBlocked = !string.IsNullOrWhiteSpace(code) && await _countryService.IsBlockedAsync(code);

            if (isBlocked)
                await _logService.LogAttemptAsync(callerIp, code ?? string.Empty, isBlocked, userAgent);

            var payload = new
            {
                IpAddress = callerIp,
                CountryCode = code,
                CountryName = name,
                Isp = isp,
                Blocked = isBlocked
            };

            return _response.Success(payload, "Check complete");
        }
    }
}
