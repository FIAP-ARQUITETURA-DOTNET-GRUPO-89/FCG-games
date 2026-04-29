using FgcGames.Application.Commands;
using FgcGames.Application.Handlers;
using FgcGames.Domain.Entities;
using FgcGames.Domain.Enum;
using FgcGames.Domain.Interfaces.Repositories;
using FgcGames.Domain.ValueObjects;
using FgcGames.Shared.Exceptions;
using NSubstitute;
using Shouldly;

namespace FgcGames.UnitTests.Application.Handlers;

public class DeleteUserHandlerTests
{
    private readonly Microsoft.Extensions.Logging.ILogger<DeleteUserHandler> _logger;
    private readonly IUsuarioRepository _repository;
    private readonly DeleteUserHandler _sut;

    public DeleteUserHandlerTests()
    {
        _logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<DeleteUserHandler>>();
        _repository = Substitute.For<IUsuarioRepository>();
        _sut = new DeleteUserHandler(_logger, _repository);
    }

    [Fact]
    public async Task Dado_UsuarioExistente_Quando_Inativar_Entao_DeveInativarComSucesso()
    {
        var usuario = CriarUsuario();
        var command = new DeleteUserCommand(usuario.Id);

        _repository.GetByIdAsync(command.Id).Returns(usuario);

        var result = await _sut.Handle(command);

        result.Inativo.ShouldBeTrue();
        result.Mensagem.ShouldBe($"O usuário {usuario.Nome} foi inativado com sucesso.");

        _repository.Received(1).Update(usuario);
        await _repository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Dado_UsuarioInexistente_Quando_Inativar_Entao_DeveLancarNotFoundException()
    {
        var command = new DeleteUserCommand(Guid.NewGuid());

        _repository.GetByIdAsync(command.Id).Returns((Usuario?)null);

        var exception = await Should.ThrowAsync<NotFoundException>(() => _sut.Handle(command));

        exception.Message.ShouldBe("Usuário não encontrado.");

        _repository.DidNotReceiveWithAnyArgs().Update(default!);
        await _repository.DidNotReceive().SaveChangesAsync();
    }

    private static Usuario CriarUsuario()
        => new("Nome", new DateTime(1990, 1, 1), Email.Create("user@email.com"), Senha.FromHash("Senha@123"), UserRole.User);
}
