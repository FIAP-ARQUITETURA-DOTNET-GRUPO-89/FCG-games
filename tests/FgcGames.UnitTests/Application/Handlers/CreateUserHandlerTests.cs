using FgcGames.Application.Commands;
using FgcGames.Application.Handlers;
using FgcGames.Domain.Entities;
using FgcGames.Domain.Interfaces.Repositories;
using FgcGames.Shared.Exceptions;
using NSubstitute;
using Shouldly;

namespace FgcGames.UnitTests.Application.Handlers;

public class CreateUserHandlerTests
{
    private readonly Microsoft.Extensions.Logging.ILogger<CreateUserHandler> _logger;
    private readonly IUsuarioRepository _repository;
    private readonly CreateUserHandler _sut;

    public CreateUserHandlerTests()
    {
        _logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<CreateUserHandler>>();
        _repository = Substitute.For<IUsuarioRepository>();
        _sut = new CreateUserHandler(_logger, _repository);
    }

    [Fact]
    public async Task Dado_EmailNaoExistente_Quando_CriarUsuario_Entao_DeveCriarComSucesso()
    {
        var command = new CreateUserCommand("Jonatas", "jonatas@email.com", new DateTime(1990, 1, 1), "Senha@123");

        _repository.ExistsByEmailAsync(command.Email).Returns(false);

        Usuario? capturedUser = null;
        _repository.When(r => r.Add(Arg.Any<Usuario>()))
            .Do(callInfo => capturedUser = callInfo.Arg<Usuario>());

        var result = await _sut.Handle(command);

        result.ShouldNotBeNull();
        result.Nome.ShouldBe(command.Nome);
        result.Email.ShouldBe(command.Email);

        capturedUser.ShouldNotBeNull();
        capturedUser.Nome.ShouldBe(command.Nome);
        capturedUser.Email.Endereco.ShouldBe(command.Email);

        await _repository.Received(1).ExistsByEmailAsync(command.Email);
        _repository.Received(1).Add(capturedUser);
        await _repository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Dado_EmailJaExistente_Quando_CriarUsuario_Entao_DeveLancarAlreadyExistsException()
    {
        var command = new CreateUserCommand("Jonatas", "jonatas@email.com", new DateTime(1990, 1, 1), "Senha@123");

        _repository.ExistsByEmailAsync(command.Email).Returns(true);

        var exception = await Should.ThrowAsync<AlreadyExistsException>(() => _sut.Handle(command));

        exception.Message.ShouldBe("Já existe um usuário com esse email");

        await _repository.Received(1).ExistsByEmailAsync(command.Email);
        _repository.DidNotReceiveWithAnyArgs().Add(default!);
        await _repository.DidNotReceive().SaveChangesAsync();
    }
}
