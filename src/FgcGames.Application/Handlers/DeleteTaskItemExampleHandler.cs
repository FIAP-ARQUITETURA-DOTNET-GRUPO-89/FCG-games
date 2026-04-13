using FgcGames.Application.Commands;
using FgcGames.Domain.Interfaces.Repositories;
using FgcGames.Shared.Exceptions;
using Microsoft.Extensions.Logging;

namespace FgcGames.Application.Handlers;

public interface IDeleteTaskItemExampleHandler
{
    Task Handle(DeleteTaskItemExampleCommand command);
}

public class DeleteTaskItemExampleHandler(ILogger<DeleteTaskItemExampleHandler> logger, ITaskItemExampleRepository repository): IDeleteTaskItemExampleHandler
{
    private readonly ILogger<DeleteTaskItemExampleHandler> _logger = logger;
    private readonly ITaskItemExampleRepository _repository = repository;

    public async Task Handle(DeleteTaskItemExampleCommand command)
    {
        _logger.LogInformation("Deletando TaskItemExample com Id: {Id}", command.Id);

        var task = await _repository.GetByIdAsync(command.Id);

        if (task is null)
        {
            _logger.LogWarning("TaskItemExample não encontrada para deleção. Id: {Id}", command.Id);
            throw new NotFoundException("Task não encontrada");
        }

        _repository.Delete(task);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("TaskItemExample deletada com sucesso. Id: {Id}", command.Id);
    }
}
