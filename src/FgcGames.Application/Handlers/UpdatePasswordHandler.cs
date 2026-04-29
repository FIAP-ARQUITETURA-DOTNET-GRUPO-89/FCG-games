using FgcGames.Application.Commands;
using FgcGames.Application.Interfaces;
using FgcGames.Application.Responses;
using FgcGames.Domain.Interfaces.Repositories;
using FgcGames.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace FgcGames.Application.Handlers;

public class UpdatePasswordHandler(ILogger<UpdatePasswordHandler> logger, IUsuarioRepository repository) : IUpdatePasswordHandler
{
    private readonly ILogger<UpdatePasswordHandler> _logger = logger;
    private readonly IUsuarioRepository _repository = repository;

    public async Task<UpdatePasswordResponse> Handle(UpdatePasswordCommand command)
    {
        var user = await _repository.GetByIdAsync(command.Id);

        if (user == null)
        {
            _logger.LogWarning("Tentativa de atualização de senha falhou: Usuário com ID {Id} não encontrado.", command.Id);
            throw new Exception($"Usuário com ID {command.Id} não encontrado.");
        }

        try
        {
            var senhaHash = BCrypt.Net.BCrypt.HashPassword(command.Password);
            var novaSenha = Senha.FromHash(senhaHash);

            user.AlterarSenha(novaSenha);

            _repository.Update(user);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Senha do usuário {Id} atualizada com sucesso.", user.Id);

            return new UpdatePasswordResponse("Senha atualizada com sucesso!");
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Erro de validação ao atualizar senha do usuário {Id}.", command.Id);
            throw;
        }
    }
}
