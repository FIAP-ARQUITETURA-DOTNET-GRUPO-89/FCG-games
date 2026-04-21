using FgcGames.Api.Filters;
using FgcGames.Application.Commands;
using FgcGames.Application.Handlers;
using FgcGames.Application.Queries;
using FgcGames.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace FgcGames.Api.Endpoints;

public static class CRUDExampleEndPoints
{
    public static void MapCRUDExampleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/crud-example").WithTags("CRUD Example");

        var taskGroup = group.MapGroup("/task-items");

        taskGroup.MapPost("/", CreateTask)
            .AddEndpointFilter<ValidationFilter<CreateTaskItemExampleCommand>>()
            .WithSummary("Cria uma nova task")
            .WithDescription("Endpoint responsável por criar uma nova task.")
            .Produces<CreateTaskItemExampleResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status500InternalServerError)
            .RequireAuthorization("Admin");

        taskGroup.MapGet("/{id:int}", GetTaskById)
            .AddEndpointFilter<ValidationFilter<GetTaskItemByIdExampleQuery>>()
            .WithSummary("Busca uma task por Id")
            .WithDescription("Endpoint responsável por retornar uma task pelo Id.")
            .Produces<GetTaskItemByIdExampleResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .RequireAuthorization("User");

        taskGroup.MapPut("/{id:int}", UpdateTask)
            .AddEndpointFilter<ValidationFilter<UpdateTaskItemExampleCommand>>()
            .WithSummary("Atualiza uma task")
            .WithDescription("Endpoint responsável por atualizar uma task existente.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status500InternalServerError);

        taskGroup.MapDelete("/{id:int}", DeleteTask)
            .AddEndpointFilter<ValidationFilter<DeleteTaskItemExampleCommand>>()
            .WithSummary("Remove uma task")
            .WithDescription("Endpoint responsável por deletar uma task existente.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> CreateTask(CreateTaskItemExampleCommand command, [FromServices] ICreateTaskItemExampleHandler handler)
    {
        var result = await handler.Handle(command);
        return Results.Created($"/crud-example/task-items/{result.Id}", result);
    }

    private static async Task<IResult> GetTaskById([AsParameters] GetTaskItemByIdExampleQuery query, [FromServices] IGetTaskItemByIdHandler handler)
    {
        var result = await handler.Handle(query.Id);
        return Results.Ok(result);
    }

    private static async Task<IResult> UpdateTask(int id, UpdateTaskItemExampleCommand command, [FromServices] IUpdateTaskItemExampleHandler handler)
    {
        await handler.Handle(id, command);
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteTask([AsParameters] DeleteTaskItemExampleCommand command, [FromServices] IDeleteTaskItemExampleHandler handler)
    {
        await handler.Handle(command);
        return Results.NoContent();
    }
}
