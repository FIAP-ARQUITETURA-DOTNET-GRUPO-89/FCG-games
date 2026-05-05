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
            .AllowAnonymous()
            .AddEndpointFilter<ValidationFilter<CreateUserCommand>>()
            .WithSummary("Cria um novo usuário")
            .WithDescription("Endpoint responsável por criar um novo usuário com perfil padrão 'User'.")
            .Produces<CreateUserResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict);

        group.MapPost("/admin", CreateAdminUser)
            .RequireAuthorization("Admin")
            .AddEndpointFilter<ValidationFilter<CreateUserCommand>>()
            .WithSummary("Cria um novo usuário administrador")
            .Produces<CreateUserResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest);

        group.MapGet("/{id:Guid}", GetUserById)
            .RequireAuthorization("Admin")
            .WithSummary("Obtém um usuário pelo ID")
            .Produces<GetUserByIdResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", GetAllUsers)
            .RequireAuthorization("Admin")
            .AddEndpointFilter<ValidationFilter<GetAllUsersQuery>>()
            .WithSummary("Lista todos os usuários")
            .Produces<PagedResponse<GetAllUsersResponse>>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest);

        group.MapGet("/busca", GetUsersByName)
            .RequireAuthorization()
            .AddEndpointFilter<ValidationFilter<GetUsersByNameQuery>>()
            .WithSummary("Busca usuários por nome")
            .Produces<GetUsersByNameResponse>(StatusCodes.Status200OK);

        group.MapPut("/{id:Guid}", UpdateUser)
            .RequireAuthorization()
            .AddEndpointFilter<ValidationFilter<UpdateUserCommand>>()
            .WithSummary("Atualiza um usuário")
            .Produces<UpdateUserResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPatch("/{id:Guid}/role", UpdateUserRole)
            .RequireAuthorization("Admin")
            .AddEndpointFilter<ValidationFilter<UpdateUserRoleCommand>>()
            .WithSummary("Atualiza a role de um usuário")
            .Produces<UpdateUserResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPatch("/{id:Guid}/password", UpdatePassword)
            .RequireAuthorization()
            .AddEndpointFilter<ValidationFilter<UpdatePasswordCommand>>()
            .WithSummary("Atualiza a senha de um usuário")
            .Produces<UpdatePasswordResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:Guid}", DeleteUser)
            .RequireAuthorization("Admin")
            .AddEndpointFilter<ValidationFilter<DeleteUserCommand>>()
            .WithSummary("Remove um usuário")
            .Produces<DeleteUserResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> CreateUser(CreateUserCommand command, [FromServices] ICreateUserHandler handler)
    {
        var result = await handler.Handle(command with { Role = Domain.Enum.UserRole.User });
        return Results.Created($"/usuarios/{result.Id}", result);
    }

    private static async Task<IResult> CreateAdminUser(CreateUserCommand command, [FromServices] ICreateUserHandler handler)
    {
        var result = await handler.Handle(command with { Role = Domain.Enum.UserRole.Admin });
        return Results.Created($"/usuarios/{result.Id}", result);
    }

    private static async Task<IResult> GetUserById(Guid id, [FromServices] IGetUserByIdHandler handler)
    {
        var result = await handler.Handle(new GetUserByIdQuery(id));
        return result is not null ? Results.Ok(result) : CreateNotFoundProblem(id);
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
        var result = await handler.Handle(command with { Id = id });
        return result is not null ? Results.Ok(result) : CreateNotFoundProblem(id);
    }

    private static async Task<IResult> UpdateUserRole(Guid id, UpdateUserRoleCommand command, [FromServices] IUpdateUserRoleHandler handler)
    {
        var result = await handler.Handle(command with { Id = id });
        return result is not null ? Results.Ok(result) : CreateNotFoundProblem(id);
    }

    private static async Task<IResult> UpdatePassword(Guid id, UpdatePasswordCommand command, [FromServices] IUpdatePasswordHandler handler)
    {
        var result = await handler.Handle(command with { Id = id });
        return result is not null ? Results.Ok(result) : CreateNotFoundProblem(id);
    }

    private static async Task<IResult> DeleteUser([AsParameters] DeleteUserCommand command, [FromServices] IDeleteUserHandler handler)
    {
        var result = await handler.Handle(command);
        return result is not null ? Results.Ok(result) : CreateNotFoundProblem(command.Id);
    }

    private static IResult CreateNotFoundProblem(Guid id)
    {
        return Results.Problem(
            detail: $"O usuário com o ID {id} não foi encontrado no sistema.",
            statusCode: StatusCodes.Status404NotFound,
            title: "Recurso não encontrado",
            type: "https://tools.ietf.org/html/rfc7231#section-6.5.4"
        );
    }
}
