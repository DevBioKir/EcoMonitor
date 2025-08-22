
using Microsoft.Extensions.Configuration;

namespace EcoMonitor.App.Services
{
    public class JwtTokenService
    {
        private readonly IConfiguration _config;

        public JwtTokenService(
            IConfiguration config)
        {
            _config = config;
        }


    }
}
