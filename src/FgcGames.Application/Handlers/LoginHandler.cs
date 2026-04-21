using FgcGames.Application.Commands;
using FgcGames.Application.Interfaces;
using FgcGames.Application.Responses;
using FgcGames.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace FgcGames.Application.Handlers;

public class LoginHandler(ILogger<LoginHandler> logger, IUsuarioRepository repository, ITokenService tokenService, ISenhaHasherService senhaHasher) : ILoginHandler
{
    private readonly ILogger<LoginHandler> _logger = logger;
    private readonly IUsuarioRepository _repository = repository;
    private readonly ITokenService _tokenService = tokenService;
    private readonly ISenhaHasherService _senhaHasher = senhaHasher;

    public async Task<LoginResponse> Handle(LoginCommand command)
    {
        var usuario = await _repository.ObterPorEmailAsync(command.Email);

        if(usuario is null || !_senhaHasher.VerificarSenha(command.Senha, usuario.Senha.Hash)) 
        {
            _logger.LogWarning("Tentativa de login inválida para o {Email}", command.Email);
            throw new UnauthorizedAccessException("Credenciais inválidas.");
        }

        var token = _tokenService.GenerateJwtToken(usuario.Email.Endereco, usuario.Role.ToString());

        return new LoginResponse(
            Token: token
        );
    }
}
