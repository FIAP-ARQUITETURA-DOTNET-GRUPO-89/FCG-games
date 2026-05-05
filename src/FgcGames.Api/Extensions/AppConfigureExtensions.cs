using FgcGames.Api.Endpoints;
using FgcGames.Api.Middlewares;
using FgcGames.Application.Interfaces;
using FgcGames.Infra.Database;
using FgcGames.Infra.Seed;
using Microsoft.EntityFrameworkCore;

namespace FgcGames.Api.Extensions;

public static class AppConfigureExtensions
{
    public static async Task Configure(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();

        if (!app.Environment.IsEnvironment("Testing"))
        {
            app.UseHttpsRedirection();
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
                c.RoutePrefix = "";
            });

            using var scope = app.Services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<FgcGamesContext>();
            var senhaHasher = scope.ServiceProvider.GetRequiredService<ISenhaHasherService>();

            if (db.Database.IsRelational())
            {
                var retries = 0;
                while (true)
                {
                    try
                    {
                        await db.Database.MigrateAsync();
                        await DevDatabaseSeeder.SeedAsync(db, senhaHasher);
                        break;
                    }
                    catch (Exception) when (retries++ < 5)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(3));
                    }
                }
            }
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapAuthEndpoints();
        app.MapUsuarioEndpoints();
        app.MapJogoEndpoints();
    }
}
