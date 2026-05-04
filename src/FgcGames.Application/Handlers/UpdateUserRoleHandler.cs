using FgcGames.Application.Commands;
using FgcGames.Application.Interfaces;
using FgcGames.Application.Responses;
using FgcGames.Domain.Enum;
using FgcGames.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace FgcGames.Application.Handlers;

public class UpdateUserRoleHandler(ILogger<UpdateUserRoleHandler> logger, IUsuarioRepository repository) : IUpdateUserRoleHandler
{
    private readonly ILogger<UpdateUserRoleHandler> _logger = logger;
    private readonly IUsuarioRepository _repository = repository;

    public async Task<UpdateUserRoleResponse?> Handle(UpdateUserRoleCommand command)
    {
        var user = await _repository.GetByIdAsync(command.Id);

        if (user == null)
        {
            _logger.LogWarning("Usuário {Id} não encontrado.", command.Id);
            return null;
        }

        if (!Enum.TryParse<UserRole>(command.RoleName, true, out var novaRole))
        {
            _logger.LogError("Tentativa de atribuir role inválida: {RoleName}", command.RoleName);
            throw new Exception("Role inválida.");
        }

        if (user.Role == UserRole.Admin && novaRole == UserRole.User)
        {
            _logger.LogWarning("Bloqueada tentativa de rebaixar Admin {Id} para User.", user.Id);
            throw new Exception("Não é permitido rebaixar um administrador para usuário comum através deste endpoint.");
        }

        user.AlterarRole(novaRole);
        _repository.Update(user);
        await _repository.SaveChangesAsync();

        return new UpdateUserRoleResponse(
            user.Id,
            user.Role.ToString(),
            "Role atualizada com sucesso!"
        );
    }
}
