using FgcGames.Application.Responses;
using FgcGames.Domain.Interfaces.Repositories;
using FgcGames.Shared.Exceptions;
using Microsoft.Extensions.Logging;

namespace FgcGames.Application.Handlers;

public interface IGetTaskItemByIdHandler
{
    Task<GetTaskItemByIdExampleResponse> Handle(int id);
}

public class GetTaskItemByIdExampleHandler(ILogger<GetTaskItemByIdExampleHandler> logger, ITaskItemExampleRepository repository) : IGetTaskItemByIdHandler
{
    private readonly ILogger<GetTaskItemByIdExampleHandler> _logger = logger;
    private readonly ITaskItemExampleRepository _repository = repository;

    public async Task<GetTaskItemByIdExampleResponse> Handle(int id)
    {
        _logger.LogInformation("Obtendo TaskItemExample com Id: {Id}", id);

        var task = await _repository.GetByIdAsync(id);

        if (task is null)
        {
            _logger.LogWarning("TaskItemExample não encontrada para o Id: {Id}", id);

            throw new NotFoundException("Task não encontrada");
        }

        return new GetTaskItemByIdExampleResponse(
            task.Id,
            task.Title,
            task.IsCompleted
        );
    }
}
