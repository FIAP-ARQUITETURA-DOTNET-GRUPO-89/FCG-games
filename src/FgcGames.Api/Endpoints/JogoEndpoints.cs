using FgcGames.Application.Commands;
using FgcGames.Application.Interfaces;
using FgcGames.Application.Queries;
using FgcGames.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace FgcGames.Api.Endpoints;

public static class JogoEndpoints
{
    public static void MapJogoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/jogos").WithTags("Jogos");

        group.MapPost("/", CreateJogo)
            .RequireAuthorization("Admin")
            .WithSummary("Cria um novo jogo")
            .WithDescription("Apenas administradores podem cadastrar jogos.")
            .Produces<GameResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapGet("/", GetAllJogos)
            .RequireAuthorization()
            .WithSummary("Lista todos os jogos ativos")
            .Produces<IEnumerable<GameResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:Guid}", GetJogoById)
            .RequireAuthorization()
            .WithSummary("Busca jogo por ID")
            .Produces<GameResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id:Guid}", UpdateJogo)
            .RequireAuthorization("Admin")
            .WithSummary("Atualiza um jogo")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPatch("/{id:Guid}/preco", AlterarPreco)
            .RequireAuthorization("Admin")
            .WithSummary("Altera o preço de um jogo")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id:Guid}", DeleteJogo)
            .RequireAuthorization("Admin")
            .WithSummary("Inativa um jogo")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> CreateJogo(
        CreateGameCommand command, [FromServices] ICreateGameHandler handler)
    {
        var result = await handler.Handle(command);
        return Results.Created($"/jogos/{result.Id}", result);
    }

    private static async Task<IResult> GetAllJogos(
        [FromServices] IGetAllGamesHandler handler)
    {
        var result = await handler.Handle(new GetAllGamesQuery());
        return Results.Ok(result);
    }

    private static async Task<IResult> GetJogoById(
        Guid id, [FromServices] IGetGameByIdHandler handler)
    {
        var result = await handler.Handle(new GetGameByIdQuery(id));
        return Results.Ok(result);
    }

    private static async Task<IResult> UpdateJogo(
        Guid id, UpdateGameCommand command, [FromServices] IUpdateGameHandler handler)
    {
        await handler.Handle(command with { Id = id });
        return Results.NoContent();
    }

    private static async Task<IResult> AlterarPreco(
        Guid id, UpdatePriceCommand command, [FromServices] IUpdatePriceHandler handler)
    {
        await handler.Handle(command with { Id = id });
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteJogo(
        Guid id, [FromServices] IDeleteGameHandler handler)
    {
        await handler.Handle(new DeleteGameCommand(id));
        return Results.NoContent();
    }
}
