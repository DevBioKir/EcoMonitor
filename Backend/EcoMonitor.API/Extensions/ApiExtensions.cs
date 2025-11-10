using System.Security.Claims;
using System.Text;
using EcoMonitor.Infrastracture.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EcoMonitor.API.Extensions;

public class ApiExtensions
{
    public static void AddApiAuthentication(
        IServiceCollection services,
        IOptions<JwtSettings> jwtSettings)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Value.Issuer,
                    ValidAudience = jwtSettings.Value.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Value.Key)),
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = ClaimTypes.Role
                };
            });
        
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminPolicy", policy =>
            {
                policy.RequireClaim(ClaimTypes.Role, "Admin");
            });
            
            options.AddPolicy("UserPolicy", policy =>
            {
                policy.RequireClaim(ClaimTypes.Role, "User");
            });
            
            options.AddPolicy("ManagerPolicy", policy =>
            {
                policy.RequireClaim(ClaimTypes.Role, "Manager");
            });
        });
    }
}