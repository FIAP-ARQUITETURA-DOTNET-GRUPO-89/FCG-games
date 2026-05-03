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

        group.MapGet("/{id:Guid}", GetUserById)
            .RequireAuthorization()
            .WithSummary("Obtém um usuário pelo ID")
            .Produces<GetUserByIdResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", GetAllUsers)
            .RequireAuthorization()
            .AddEndpointFilter<ValidationFilter<GetAllUsersQuery>>()
            .WithSummary("Lista todos os usuários")
            .WithDescription("Retorna uma lista paginada de usuários, podendo filtrar por ativos ou inativos.")
            .Produces<PagedResponse<GetAllUsersResponse>>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapGet("/busca", GetUsersByName)
            .RequireAuthorization()
            .AddEndpointFilter<ValidationFilter<GetUsersByNameQuery>>()
            .WithSummary("Busca usuários por nome")
            .WithDescription("Endpoint responsável por retornar usuários pelo nome.")
            .Produces<GetUsersByNameResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id:Guid}", UpdateUser)
            .RequireAuthorization()
            .AddEndpointFilter<ValidationFilter<UpdateUserCommand>>()
            .WithSummary("Atualiza um usuário")
            .WithDescription("Endpoint responsável por atualizar um usuário existente.")
            .Produces<UpdateUserResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPatch("/{id:Guid}/role", UpdateUserRole)
            .RequireAuthorization("Admin")
            .AddEndpointFilter<ValidationFilter<UpdateUserRoleCommand>>()
            .WithSummary("Atualiza a role de um usuário")
            .WithDescription("Endpoint responsável por atualizar a role de um usuário existente.")
            .Produces<UpdateUserResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPatch("/{id:Guid}/password", UpdatePassword)
            .RequireAuthorization()
            .AddEndpointFilter<ValidationFilter<UpdatePasswordCommand>>()
            .WithSummary("Atualiza a senha de um usuário")
            .WithDescription("Endpoint responsável por atualizar a senha de um usuário existente.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<UpdatePasswordResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id:Guid}", DeleteUser)
            .RequireAuthorization("Admin")
            .AddEndpointFilter<ValidationFilter<DeleteUserCommand>>()
            .WithSummary("Remove um usuário")
            .WithDescription("Endpoint responsável por deletar um usuário existente.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<DeleteUserResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> CreateUser(CreateUserCommand command, [FromServices] ICreateUserHandler handler)
    {
        var result = await handler.Handle(command);
        return Results.Created($"/usuarios/{result.Id}", result);
    }

    private static async Task<IResult> GetUserById(Guid id, [FromServices] IGetUserByIdHandler handler)
    {
        var result = await handler.Handle(new GetUserByIdQuery(id));
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> GetAllUsers([AsParameters] GetAllUsersQuery query, [FromServices] IGetAllUsersHandler handler)
    {
        var result = await handler.Handle(query);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetUsersByName([AsParameters] GetUsersByNameQuery query, [FromServices] IGetUsersByNameHandler handler)
    {
        var result = await handler.Handle(query);
        return Results.Ok(result);
    }

    private static async Task<IResult> UpdateUser(Guid id, UpdateUserCommand command, [FromServices] IUpdateUserHandler handler)
    {
        var result = await handler.Handle(command);
        return Results.Ok(result);
    }

    private static async Task<IResult> UpdateUserRole(Guid id, UpdateUserRoleCommand command, [FromServices] IUpdateUserRoleHandler handler)
    {
        var result = await handler.Handle(command);
        return Results.Ok(result);
    }

    private static async Task<IResult> UpdatePassword(Guid id, UpdatePasswordCommand command, [FromServices] IUpdatePasswordHandler handler)
    {
        var result = await handler.Handle(command with { Id = id });
        return Results.Ok(result);
    }

    private static async Task<IResult> DeleteUser([AsParameters] DeleteUserCommand command, [FromServices] IDeleteUserHandler handler)
    {
        var result = await handler.Handle(command);
        return Results.Ok(result);
    }
}
