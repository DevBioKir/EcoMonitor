using EcoMonitor.Core.Models.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EcoMonitor.Infrastracture.Authentication
{
    public class JWTService : IJWTService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public JWTService(
            IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;

        }
        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email.Value),
                new Claim(ClaimTypes.Name, $"{user.Firstname} {user.Surname}"),
                new Claim(ClaimTypes.Role, user.Role.Name)
            };

            // Добавляем Permissions как отдельные клеймы
            foreach (var permission in user.Role.Permissions)
            {
                claims.Add(new Claim("permission", permission.Code));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
