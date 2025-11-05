using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Context;
using System.Security.Claims;

namespace BlockedCountries.Infrastructure.Logging.Middleware
{
    public class SerilogUserEnricherMiddleware
    {
        private readonly RequestDelegate _next;

        public SerilogUserEnricherMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var ip = context.Connection.RemoteIpAddress?.ToString();
                var user = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? context.User?.FindFirst(ClaimTypes.Name)?.Value
                        ?? "anonymous";
                var role = context.User?.FindFirst(ClaimTypes.Role)?.Value ?? "none";

                using (LogContext.PushProperty("IpAddress", ip ?? "unknown"))
                using (LogContext.PushProperty("UserName", user))
                using (LogContext.PushProperty("UserRole", role))
                {
                    await _next(context);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error enriching Serilog context");
                await _next(context);
            }
        }
    }
}
