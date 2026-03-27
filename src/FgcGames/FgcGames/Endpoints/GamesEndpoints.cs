using FgcGames.Domain.Entities;

namespace FgcGames.Api.Endpoints
{
    public static class GamesEndpoints
    {
        public static void MapGamesEndpoints(this WebApplication app)
        {
            app.MapGet("/games", async (FgcGamesContext db) => db.Games.ToList());

            app.MapPost("/games", async (Games book, FgcGamesContext db) =>
            {
                db.Games.Add(book);
                await db.SaveChangesAsync();

                return Results.Created($"/games/{book.Id}", book);
            });
        }
    }
}
