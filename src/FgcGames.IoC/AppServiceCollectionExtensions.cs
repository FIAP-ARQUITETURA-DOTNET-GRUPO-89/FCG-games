using FgcGames.Application.Handlers;
using FgcGames.Application.Interfaces;
using FgcGames.Application.Validators;
using FgcGames.Domain.Interfaces.Repositories;
using FgcGames.Infra.Database;
using FgcGames.Infra.Repositories;
using FgcGames.Infra.Services;
using FgcGames.Shared.Settings;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FgcGames.IoC;

public static class AppServiceCollectionExtensions
{
    public static void ConfigureAppDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtSettings>().Bind(configuration.GetSection("JwtSettings"))
                .ValidateDataAnnotations()
                .ValidateOnStart();

        services.AddValidatorsFromAssemblyContaining<IValidators>();

        services.AddDbContext<FgcGamesContext>(options => options.UseNpgsql(configuration.GetConnectionString("Default")));

        // Handlers
        services.AddScoped<IGetUsersByNameHandler, GetUsersByNameHandler>();
        services.AddScoped<ICreateUserHandler, CreateUserHandler>();
        services.AddScoped<IUpdateUserHandler, UpdateUserHandler>();
        services.AddScoped<IUpdateUserRoleHandler, UpdateUserRoleHandler>();
        services.AddScoped<IUpdatePasswordHandler, UpdatePasswordHandler>();
        services.AddScoped<IDeleteUserHandler, DeleteUserHandler>();

        services.AddScoped<ILoginHandler, LoginHandler>();

        // Repositories
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();

        // Services
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ISenhaHasherService, SenhaHasherService>();
    }
}
