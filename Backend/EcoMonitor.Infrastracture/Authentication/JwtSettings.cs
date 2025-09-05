namespace EcoMonitor.Infrastracture.Authentication
{
    public class JwtSettings
    {
        public string Key { get; set; } = string.Empty; // Secret Key
        public string Issuer { get; set; } = string.Empty; // Who issued it
        public string Audience { get; set; } = string.Empty; // Audience
        public int ExpiresInMinutes { get; set; } = 60; // Lifespan
    }
}
