using FgcGames.Application.Commands;
using FgcGames.Application.Responses;
using FgcGames.Domain.Entities;
using FgcGames.Domain.Interfaces.Repositories;
using FgcGames.Shared.Exceptions;
using Microsoft.Extensions.Logging;

namespace FgcGames.Application.Handlers;

public interface ICreateTaskItemExampleHandler
{
    Task<CreateTaskItemExampleResponse> Handle(CreateTaskItemExampleCommand command);
}

public class CreateTaskItemExampleHandler(ILogger<CreateTaskItemExampleHandler> logger, ITaskItemExampleRepository repository) : ICreateTaskItemExampleHandler
{
    private readonly ILogger<CreateTaskItemExampleHandler> _logger = logger;
    private readonly ITaskItemExampleRepository _repository = repository;

    public async Task<CreateTaskItemExampleResponse> Handle(CreateTaskItemExampleCommand command)
    {
        _logger.LogInformation("Criando TaskItemExample com título: {Title}", command.Title);

        var alreadyExists = await _repository.ExistsByTitleAsync(command.Title);
        if (alreadyExists)
        {
            _logger.LogWarning("Já existe uma Task com o título: {Title}", command.Title);

            throw new AlreadyExistsException("Já existe uma task com esse título");
        }

        var task = new TaskItemExample(command.Title);

        _repository.Add(task);

        await _repository.SaveChangesAsync();

        return new CreateTaskItemExampleResponse(
            task.Id,
            task.Title,
            task.IsCompleted
        );
    }
}
