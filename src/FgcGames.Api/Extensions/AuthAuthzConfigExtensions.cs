using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace FgcGames.Api.Extensions;

public static class AuthAuthzConfigExtensions
{
    public static IServiceCollection AuthAuthzConfig(this IServiceCollection services)
    {
        const string issuer = "FgcGames-Issuer";
        const string securityKey = "FgcGames_Secret_Key_2026_High_Security_Token";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey))
            };
        });

        services.AddAuthorizationBuilder()
                .AddPolicy("Admin", policy => policy.RequireRole("Admin"))
                .AddPolicy("Usuario", policy => policy.RequireRole("Usuario"));

        return services;
    }
}
