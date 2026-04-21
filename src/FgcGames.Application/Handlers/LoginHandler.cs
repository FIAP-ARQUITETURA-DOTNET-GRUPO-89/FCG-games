using FgcGames.Application.Commands;
using FgcGames.Application.Interfaces;
using FgcGames.Application.Responses;
using FgcGames.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace FgcGames.Application.Handlers;

public class LoginHandler(ILogger<LoginHandler> logger, IUsuarioRepository repository, ITokenService tokenService) : ILoginHandler
{
    private readonly ILogger<LoginHandler> _logger = logger;
    private readonly IUsuarioRepository _repository = repository;
    private readonly ITokenService _tokenService;

    public async Task<LoginResponse> Handle(LoginCommand command)
    {
        var token = _tokenService.GenerateJwtToken("email@email.com", "Admin");

        return new LoginResponse(
            Token: token
        );
    }
}
