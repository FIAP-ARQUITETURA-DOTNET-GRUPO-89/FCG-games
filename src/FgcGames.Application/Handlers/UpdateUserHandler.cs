using FgcGames.Application.Commands;
using FgcGames.Application.Interfaces;
using FgcGames.Application.Responses;
using FgcGames.Domain.Interfaces.Repositories;
using FgcGames.Shared.Exceptions;
using Microsoft.Extensions.Logging;

namespace FgcGames.Application.Handlers;

public class UpdateUserHandler(ILogger<UpdateUserHandler> logger, IUsuarioRepository repository) : IUpdateUserHandler
{
    private readonly ILogger<UpdateUserHandler> _logger = logger;
    private readonly IUsuarioRepository _repository = repository;

    public async Task<UpdateUserResponse> Handle(UpdateUserCommand command)
    {
        var user = await _repository.GetByIdAsync(command.Id);

        if (user == null)
        {
            _logger.LogWarning("Tentativa de atualização falhou: Usuário com ID {Id} não encontrado.", command.Id);
            throw new NotFoundException($"Usuário com ID {command.Id} não encontrado.");
        }

        try
        {
            user.AtualizarPerfil(command.Nome, command.DataNascimento);

            _repository.Update(user);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Usuário {Id} updated com sucesso.", user.Id);

            return new UpdateUserResponse(
                user.Id,
                user.Nome,
                user.DataNascimento,
                "Perfil atualizado com sucesso!"
            );
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Erro de validação ao atualizar usuário {Id}.", command.Id);
            throw;
        }
    }
}
