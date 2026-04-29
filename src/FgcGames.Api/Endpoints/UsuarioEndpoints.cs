using FgcGames.Api.Filters;
using FgcGames.Application.Commands;
using FgcGames.Application.Interfaces;
using FgcGames.Application.Queries;
using FgcGames.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace FgcGames.Api.Endpoints;

public static class UsuarioEndpoints
{
    public static void MapUsuarioEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/usuarios").WithTags("Usuarios");


        group.MapPost("/", CreateUser)
            .AddEndpointFilter<ValidationFilter<CreateUserCommand>>()
            .WithSummary("Cria um novo usuário")
            .WithDescription("Endpoint responsável por criar um novo usuário.")
            .Produces<CreateUserResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapGet("/", GetUsersByName)
            .AddEndpointFilter<ValidationFilter<GetUsersByNameQuery>>()
            .WithSummary("Busca usuários por nome")
            .WithDescription("Endpoint responsável por retornar usuários pelo nome.")
            .Produces<GetUsersByNameResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id:Guid}", UpdateUser)
            .AddEndpointFilter<ValidationFilter<UpdateUserCommand>>()
            .WithSummary("Atualiza um usuário")
            .WithDescription("Endpoint responsável por atualizar um usuário existente.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPatch("/{id:Guid}/role", UpdateUserRole)
            .AddEndpointFilter<ValidationFilter<UpdateUserRoleCommand>>()
            .WithSummary("Atualiza a role de um usuário")
            .WithDescription("Endpoint responsável por atualizar a role de um usuário existente.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPatch("/{id:Guid}/password", UpdatePassword)
            .AddEndpointFilter<ValidationFilter<UpdatePasswordCommand>>()
            .WithSummary("Atualiza a senha de um usuário")
            .WithDescription("Endpoint responsável por atualizar a senha de um usuário existente.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id:Guid}", DeleteUser)
            .AddEndpointFilter<ValidationFilter<DeleteUserCommand>>()
            .WithSummary("Remove um usuário")
            .WithDescription("Endpoint responsável por deletar um usuário existente.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> CreateUser(CreateUserCommand command, [FromServices] ICreateUserHandler handler)
    {
        var result = await handler.Handle(command);
        return Results.Created($"/usuarios/{result.Id}", result);
    }

    private static async Task<IResult> GetUsersByName([AsParameters] GetUsersByNameQuery query, [FromServices] IGetUsersByNameHandler handler)
    {
        var result = await handler.Handle(query);
        return Results.Ok(result);
    }

    private static async Task<IResult> UpdateUser(Guid id, UpdateUserCommand command, [FromServices] IUpdateUserHandler handler)
    {
        await handler.Handle(command);
        return Results.NoContent();
    }

    private static async Task<IResult> UpdateUserRole(Guid id, UpdateUserRoleCommand command, [FromServices] IUpdateUserRoleHandler handler)
    {
        await handler.Handle(command);
        return Results.NoContent();
    }

    private static async Task<IResult> UpdatePassword(Guid id, UpdatePasswordCommand command, [FromServices] IUpdatePasswordHandler handler)
    {
        await handler.Handle(command);
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteUser([AsParameters] DeleteUserCommand command, [FromServices] IDeleteUserHandler handler)
    {
        await handler.Handle(command);
        return Results.NoContent();
    }
}
