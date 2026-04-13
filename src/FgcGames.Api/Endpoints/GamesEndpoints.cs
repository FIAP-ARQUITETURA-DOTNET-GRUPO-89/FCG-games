using FgcGames.Domain.Entities;
using FgcGames.Infra.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FgcGames.Api.Endpoints;

public static class GamesEndpoints
{
    public static void MapGamesEndpoints(this WebApplication app)
    {
        app.MapGet("/games", async ([FromServices] FgcGamesContext db) =>
            await db.Games.ToListAsync()
        );

        app.MapPost("/games", async (Games book, [FromServices] FgcGamesContext db) =>
        {
            db.Games.Add(book);
            await db.SaveChangesAsync();

            return Results.Created($"/games/{book.Id}", book);
        });
    }
}
