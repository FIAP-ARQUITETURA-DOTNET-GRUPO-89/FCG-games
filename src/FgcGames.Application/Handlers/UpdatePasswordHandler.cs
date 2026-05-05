using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FgcGames.Application.Commands;
using FgcGames.Application.Interfaces;
using FgcGames.Application.Responses;
using FgcGames.Domain.Interfaces.Repositories;
using FgcGames.Domain.ValueObjects;
using FgcGames.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FgcGames.Application.Handlers;

public class UpdatePasswordHandler(ILogger<UpdatePasswordHandler> logger, IUsuarioRepository repository, IHttpContextAccessor httpContextAccessor) : IUpdatePasswordHandler
{
    private readonly ILogger<UpdatePasswordHandler> _logger = logger;
    private readonly IUsuarioRepository _repository = repository;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<UpdatePasswordResponse> Handle(UpdatePasswordCommand command)
    {
        var userEmailFromToken = _httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                                 ?? _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userEmailFromToken))
        {
            _logger.LogWarning("Tentativa de alteração de senha sem e-mail no token.");
            throw new UnauthorizedAccessException("Usuário não autenticado.");
        }

        var user = await _repository.GetByIdAsync(command.Id);

        if (user == null)
        {
            _logger.LogWarning("Usuário com ID {Id} não encontrado.", command.Id);
            throw new NotFoundException("Usuário não encontrado.");
        }

        if (!string.Equals(user.Email.ToString(), userEmailFromToken, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogError("VIOLAÇÃO: {AuthEmail} tentou alterar senha de {TargetEmail}.", userEmailFromToken, user.Email);
            throw new HttpRequestException("Você só pode alterar a sua própria senha.", null, System.Net.HttpStatusCode.Forbidden);
        }

        try
        {
            Senha.ValidarTextoPuro(command.Password);

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
