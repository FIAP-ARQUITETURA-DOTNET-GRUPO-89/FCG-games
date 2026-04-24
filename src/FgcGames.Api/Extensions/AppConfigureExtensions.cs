using FgcGames.Api.Endpoints;
using FgcGames.Api.Middlewares;
using FgcGames.Application.Interfaces;
using FgcGames.Infra.Database;
using FgcGames.Infra.Seed;
using Microsoft.EntityFrameworkCore;

namespace FgcGames.Api.Extensions;

public static class AppConfigureExtensions
{
    public static async Task ConfigureAsync(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseHttpsRedirection();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
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
                await db.Database.MigrateAsync();

                await DevDatabaseSeeder.SeedAsync(db, senhaHasher);
            }
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapDefaultEndpoints();

        app.MapCRUDExampleEndpoints();
        app.MapAuthEndpoints();
        app.MapUsuarioEndpoints();
        app.MapJogoEndpoints();
    }
}
