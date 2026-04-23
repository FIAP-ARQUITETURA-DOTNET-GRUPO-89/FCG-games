using FgcGames.Application.Commands;
using FgcGames.Application.Interfaces;
using FgcGames.Application.Responses;
using FgcGames.Domain.Interfaces.Repositories;
using FgcGames.Shared.Exceptions;
using Microsoft.Extensions.Logging;

namespace FgcGames.Application.Handlers;

public class DeleteUserHandler(ILogger<DeleteUserHandler> logger, IUsuarioRepository repository) : IDeleteUserHandler
{
    private readonly ILogger<DeleteUserHandler> _logger = logger;
    private readonly IUsuarioRepository _repository = repository;

    public async Task<DeleteUserResponse> Handle(DeleteUserCommand command)
    {
        var user = await _repository.GetByIdAsync(command.Id);

        if (user == null)
        {
            _logger.LogWarning("Tentativa de inativar usuário inexistente: {Id}", command.Id);
            throw new NotFoundException("Usuário não encontrado.");
        }

        user.Inativar();

        _repository.Update(user);
        await _repository.SaveChangesAsync();

        return new DeleteUserResponse(
        user.Id,
        user.Nome,
        user.Inativo,
        $"O usuário {user.Nome} foi inativado com sucesso."
    );
    }
}
