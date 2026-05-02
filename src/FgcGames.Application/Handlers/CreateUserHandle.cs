using FgcGames.Application.Commands;
using FgcGames.Application.Interfaces;
using FgcGames.Application.Responses;
using FgcGames.Domain.Entities;
using FgcGames.Domain.Interfaces.Repositories;
using FgcGames.Domain.ValueObjects;
using FgcGames.Shared.Exceptions;
using Microsoft.Extensions.Logging;

namespace FgcGames.Application.Handlers;

public class CreateUserHandler(ILogger<CreateUserHandler> logger, IUsuarioRepository repository) : ICreateUserHandler
{
    private readonly ILogger<CreateUserHandler> _logger = logger;
    private readonly IUsuarioRepository _repository = repository;

    public async Task<CreateUserResponse> Handle(CreateUserCommand command)
    {
        var email = new Email(command.Email);
        var senha = new Senha(command.Senha);

        var senhaHash = BCrypt.Net.BCrypt.HashPassword(command.Senha);
        var senha = Senha.FromHash(senhaHash);

        var alreadyExists = await _repository.ExistsByEmailAsync(command.Email);
        if (alreadyExists)
        {
            _logger.LogWarning("Já existe um usuário com o email: {Email}", email);

            throw new AlreadyExistsException("Já existe um usuário com esse email");
        }

        var user = new Usuario(
            command.Nome,
            command.DataNascimento,
            email,
            senha,
            0
        );

        _repository.Add(user);

        await _repository.SaveChangesAsync();

        return new CreateUserResponse(
            user.Id,
            user.Nome,
            user.DataNascimento,
            user.Email.Endereco
        );
    }
}
