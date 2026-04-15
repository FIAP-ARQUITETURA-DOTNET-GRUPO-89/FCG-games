namespace FgcGames.Api.Endpoints;

public static class JogoEndpoints
{
    public static void MapJogoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/jogos").WithTags("Jogos");
    }
}
