using FgcGames.Application.Handlers;
using FgcGames.Application.Validators;
using FgcGames.Domain.Interfaces.Repositories;
using FgcGames.Infra.Database;
using FgcGames.Infra.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FgcGames.IoC;

public static class AppServiceCollectionExtensions
{
    public static void ConfigureAppDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatorsFromAssemblyContaining<IValidators>();

        services.AddDbContext<FgcGamesContext>(options => options.UseNpgsql(configuration.GetConnectionString("Default")));

        // Handlers
        services.AddScoped<ICreateTaskItemExampleHandler, CreateTaskItemExampleHandler>();
        services.AddScoped<IGetTaskItemByIdHandler, GetTaskItemByIdExampleHandler>();
        services.AddScoped<IUpdateTaskItemExampleHandler, UpdateTaskItemExampleHandler>();
        services.AddScoped<IDeleteTaskItemExampleHandler, DeleteTaskItemExampleHandler>();

        // Repositories
        services.AddScoped<ITaskItemExampleRepository, TaskItemExampleRepository>();
    }
}
