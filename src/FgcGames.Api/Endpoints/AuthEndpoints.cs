using FgcGames.Api.Filters;
using FgcGames.Application.Commands;
using FgcGames.Application.Interfaces;
using FgcGames.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace FgcGames.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth").WithTags("Auth");

        group.MapPost("/", Login)
            .AddEndpointFilter<ValidationFilter<LoginCommand>>()
            .WithSummary("Realiza a autenticação do usuário")
            .WithDescription("Autentica um usuário com base nas credenciais fornecidas e retorna um token de acesso.")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> Login(LoginCommand command, [FromServices] ILoginHandler handler)
    {
        var result = await handler.Handle(command);

        if (result is null)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(result);
    }
}
