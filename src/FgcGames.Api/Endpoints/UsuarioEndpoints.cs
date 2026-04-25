using FgcGames.Api.Filters;
using FgcGames.Application.Commands;
using FgcGames.Application.Handlers;
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

        var userGroup = group.MapGroup("/users");

        userGroup.MapPost("/", CreateUser)
            .AddEndpointFilter<ValidationFilter<CreateUserCommand>>()
            .WithSummary("Cria um novo usuário")
            .WithDescription("Endpoint responsável por criar um novo usuário.")
            .Produces<CreateUserResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status500InternalServerError);

        //userGroup.MapGet("/{id:int}", GetUserById)
        //    .AddEndpointFilter<ValidationFilter<GetTaskItemByIdExampleQuery>>()
        //    .WithSummary("Busca uma task por Id")
        //    .WithDescription("Endpoint responsável por retornar uma task pelo Id.")
        //    .Produces<GetTaskItemByIdExampleResponse>(StatusCodes.Status200OK)
        //    .ProducesValidationProblem(StatusCodes.Status400BadRequest)
        //    .Produces(StatusCodes.Status404NotFound)
        //    .Produces(StatusCodes.Status500InternalServerError);

        userGroup.MapPut("/{id:int}", UpdateUser)
            .AddEndpointFilter<ValidationFilter<UpdateUserCommand>>()
            .WithSummary("Atualiza um usuário")
            .WithDescription("Endpoint responsável por atualizar um usuário existente.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status500InternalServerError);

        userGroup.MapPut("/{id:int}/password", UpdatePassword)
            .AddEndpointFilter<ValidationFilter<UpdatePasswordCommand>>()
            .WithSummary("Atualiza a senha de um usuário")
            .WithDescription("Endpoint responsável por atualizar a senha de um usuário existente.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status500InternalServerError);

        userGroup.MapDelete("/{id:int}", DeleteUser)
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

    //private static async Task<IResult> GetUserById([AsParameters] GetUserByIdQuery query, [FromServices] IGetUserByIdHandler handler)
    //{
    //    var result = await handler.Handle(query.Id);
    //    return Results.Ok(result);
    //}

    private static async Task<IResult> UpdateUser(int id, UpdateUserCommand command, [FromServices] IUpdateUserHandler handler)
    {
        await handler.Handle(command);
        return Results.NoContent();
    }

    private static async Task<IResult> UpdatePassword(int id, UpdatePasswordCommand command, [FromServices] IUpdatePasswordHandler handler)
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
