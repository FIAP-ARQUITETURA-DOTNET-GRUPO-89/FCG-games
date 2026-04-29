using FgcGames.Api.Filters;
using FgcGames.IoC;
using Microsoft.OpenApi;

namespace FgcGames.Api.Extensions;

public static class ConfigureServicesExtensions
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication();
        services.AddAuthorization();
        services.AuthnAuthzConfig();

        services.AddEndpointsApiExplorer();

        services.AddScoped(typeof(ValidationFilter<>));
        services.ConfigureAppDependencies(configuration);
    }
}
