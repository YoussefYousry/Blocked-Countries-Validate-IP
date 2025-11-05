namespace BlockedCountries.Domain.Models.Logging
{
    public class LogEntry
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Level { get; set; } = "Information";
        public string Message { get; set; } = string.Empty;
        public string? Exception { get; set; }
        public string? SourceContext { get; set; }
        public string? UserName { get; set; }
        public string? IpAddress { get; set; }
        public string? MachineName { get; set; }
        public string? UserRole { get; set; }
    }
}
