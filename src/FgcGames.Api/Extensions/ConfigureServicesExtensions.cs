using FgcGames.Api.Filters;
using FgcGames.IoC;

namespace FgcGames.Api.Extensions;

public static class ConfigureServicesExtensions
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication();
        services.AddAuthorization();
        services.AuthAuthzConfig();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddScoped(typeof(ValidationFilter<>));
        services.ConfigureAppDependencies(configuration);
    }
}
