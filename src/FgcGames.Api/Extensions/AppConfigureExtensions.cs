using FgcGames.Api.Endpoints;
using FgcGames.Api.Middlewares;
using FgcGames.Infra.Database;
using Microsoft.EntityFrameworkCore;

namespace FgcGames.Api.Extensions;

public static class AppConfigureExtensions
{
    public static void Configure(this WebApplication app)
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

            if (db.Database.IsRelational())
            {
                db.Database.Migrate();
            }
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapGamesEndpoints();
        app.MapCRUDExampleEndpoints();
    }
}
