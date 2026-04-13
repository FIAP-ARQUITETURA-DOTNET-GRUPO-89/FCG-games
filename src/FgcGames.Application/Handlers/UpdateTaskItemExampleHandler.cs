using FgcGames.Application.Commands;
using FgcGames.Domain.Interfaces.Repositories;
using FgcGames.Shared.Exceptions;
using Microsoft.Extensions.Logging;

namespace FgcGames.Application.Handlers;

public interface IUpdateTaskItemExampleHandler
{
    Task Handle(int id, UpdateTaskItemExampleCommand command);
}

public class UpdateTaskItemExampleHandler(ILogger<UpdateTaskItemExampleHandler> logger, ITaskItemExampleRepository repository) : IUpdateTaskItemExampleHandler
{
    private readonly ILogger<UpdateTaskItemExampleHandler> _logger = logger;
    private readonly ITaskItemExampleRepository _repository = repository;

    public async Task Handle(int id, UpdateTaskItemExampleCommand command)
    {
        _logger.LogInformation("Atualizando TaskItemExample com Id: {Id}", id);

        var task = await _repository.GetByIdAsync(id);
        if (task is null)
        {
            _logger.LogWarning("Task não encontrada. Id: {Id}", id);
            throw new NotFoundException("Task não encontrada");
        }

        var alreadyExists = await _repository.ExistsByTitleAsync(command.Title);
        if (alreadyExists && !string.Equals(task.Title, command.Title, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Tentativa de duplicação de título. Id: {Id}, Title: {Title}", id, command.Title);
            throw new AlreadyExistsException("Já existe uma task com esse título");
        }

        task.UpdateTitle(command.Title);
        if (command.IsCompleted && !task.IsCompleted)
        {
            task.Complete();
        }

        _repository.Update(task);

        await _repository.SaveChangesAsync();
    }
}
