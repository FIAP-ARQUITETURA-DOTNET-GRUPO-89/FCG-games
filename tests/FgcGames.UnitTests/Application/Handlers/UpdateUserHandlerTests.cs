using FgcGames.Application.Commands;
using FgcGames.Application.Handlers;
using FgcGames.Domain.Entities;
using FgcGames.Domain.Enum;
using FgcGames.Domain.Interfaces.Repositories;
using FgcGames.Domain.ValueObjects;
using NSubstitute;
using Shouldly;

namespace FgcGames.UnitTests.Application.Handlers;

public class UpdateUserHandlerTests
{
    private readonly Microsoft.Extensions.Logging.ILogger<UpdateUserHandler> _logger;
    private readonly IUsuarioRepository _repository;
    private readonly UpdateUserHandler _sut;

    public UpdateUserHandlerTests()
    {
        _logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<UpdateUserHandler>>();
        _repository = Substitute.For<IUsuarioRepository>();
        _sut = new UpdateUserHandler(_logger, _repository);
    }

    [Fact]
    public async Task Dado_UsuarioExistente_Quando_AtualizarPerfil_Entao_DeveAtualizarComSucesso()
    {
        var usuario = CriarUsuario();
        var command = new UpdateUserCommand(usuario.Id, "Novo Nome", new DateTime(1995, 1, 1));

        _repository.GetByIdAsync(command.Id).Returns(usuario);

        var result = await _sut.Handle(command);

        result.Nome.ShouldBe(command.Nome);
        result.DataNascimento.ShouldBe(command.DataNascimento);
        result.Mensagem.ShouldBe("Perfil atualizado com sucesso!");

        await _repository.Received(1).GetByIdAsync(command.Id);
        _repository.Received(1).Update(usuario);
        await _repository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Dado_UsuarioInexistente_Quando_AtualizarPerfil_Entao_DeveLancarException()
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "Novo Nome", new DateTime(1995, 1, 1));

        _repository.GetByIdAsync(command.Id).Returns((Usuario?)null);

        var exception = await Should.ThrowAsync<Exception>(() => _sut.Handle(command));

        exception.Message.ShouldBe($"Usuário com ID {command.Id} não encontrado.");

        await _repository.Received(1).GetByIdAsync(command.Id);
        _repository.DidNotReceiveWithAnyArgs().Update(default!);
        await _repository.DidNotReceive().SaveChangesAsync();
    }

    [Fact]
    public async Task Dado_NomeInvalido_Quando_AtualizarPerfil_Entao_DeveLancarArgumentException()
    {
        var usuario = CriarUsuario();
        var command = new UpdateUserCommand(usuario.Id, "", new DateTime(1995, 1, 1));

        _repository.GetByIdAsync(command.Id).Returns(usuario);

        await Should.ThrowAsync<ArgumentException>(() => _sut.Handle(command));

        _repository.DidNotReceiveWithAnyArgs().Update(default!);
        await _repository.DidNotReceive().SaveChangesAsync();
    }

    private static Usuario CriarUsuario()
        => new("Nome", new DateTime(1990, 1, 1), Email.Create("user@email.com"), Senha.Create("Senha@123"), UserRole.User);
}
