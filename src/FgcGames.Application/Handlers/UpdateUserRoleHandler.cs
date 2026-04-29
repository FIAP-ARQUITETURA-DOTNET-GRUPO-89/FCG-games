using FgcGames.Application.Commands;
using FgcGames.Application.Interfaces;
using FgcGames.Application.Responses;
using FgcGames.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace FgcGames.Application.Handlers;

public class UpdateUserRoleHandler(ILogger<UpdateUserRoleHandler> logger, IUsuarioRepository repository) : IUpdateUserRoleHandler
{
    private readonly ILogger<UpdateUserRoleHandler> _logger = logger;
    private readonly IUsuarioRepository _repository = repository;

    public async Task<UpdateUserRoleResponse> Handle(UpdateUserRoleCommand command)
    {
        var user = await _repository.GetByIdAsync(command.Id);

        if (user == null)
        {
            _logger.LogWarning("Tentativa de atualização de role falhou: Usuário com ID {Id} não encontrado.", command.Id);
            throw new Exception($"Usuário com ID {command.Id} não encontrado.");
        }

        user.AlterarRole(command.Role);

        _repository.Update(user);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Role do usuário {Id} atualizada com sucesso para {Role}.", user.Id, user.Role);

        return new UpdateUserRoleResponse(
            user.Id,
            user.Role,
            "Role atualizada com sucesso!"
        );
    }
}
