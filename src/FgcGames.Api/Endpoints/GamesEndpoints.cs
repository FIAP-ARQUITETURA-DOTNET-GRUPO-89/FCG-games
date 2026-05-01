using FgcGames.Domain.Entities;

namespace FgcGames.Api.Endpoints;

public static class GamesEndpoints
{
    public static void MapGamesEndpoints(this WebApplication app)
    {
        app.MapGet("/games", () => Results.Ok(Array.Empty<Games>()));

        app.MapPost("/games", (Games book) =>
            Results.Created($"/games/{book.Id}", book));
    }
}
