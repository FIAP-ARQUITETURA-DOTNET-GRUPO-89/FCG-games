using FgcGames.Application.Commands;
using FgcGames.Application.Handlers;
using FgcGames.Domain.Entities;
using FgcGames.Domain.Enum;
using FgcGames.Domain.Interfaces.Repositories;
using FgcGames.Domain.ValueObjects;
using NSubstitute;
using Shouldly;

namespace FgcGames.UnitTests.Application.Handlers;

public class UpdatePasswordHandlerTests
{
    private readonly Microsoft.Extensions.Logging.ILogger<UpdatePasswordHandler> _logger;
    private readonly IUsuarioRepository _repository;
    private readonly UpdatePasswordHandler _sut;

    public UpdatePasswordHandlerTests()
    {
        _logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<UpdatePasswordHandler>>();
        _repository = Substitute.For<IUsuarioRepository>();
        _sut = new UpdatePasswordHandler(_logger, _repository);
    }

    [Fact]
    public async Task Dado_UsuarioExistente_Quando_AtualizarSenha_Entao_DeveAtualizarComHash()
    {
        var usuario = CriarUsuario();
        var command = new UpdatePasswordCommand(usuario.Id, "Senha@123");

        _repository.GetByIdAsync(command.Id).Returns(usuario);

        var result = await _sut.Handle(command);

        result.Mensagem.ShouldBe("Senha atualizada com sucesso!");
        usuario.Senha.Hash.ShouldNotBe(command.Password);
        usuario.Senha.Hash.StartsWith("$2").ShouldBeTrue();

        await _repository.Received(1).GetByIdAsync(command.Id);
        _repository.Received(1).Update(usuario);
        await _repository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Dado_UsuarioInexistente_Quando_AtualizarSenha_Entao_DeveLancarException()
    {
        var command = new UpdatePasswordCommand(Guid.NewGuid(), "Senha@123");

        _repository.GetByIdAsync(command.Id).Returns((Usuario?)null);

        var exception = await Should.ThrowAsync<Exception>(() => _sut.Handle(command));

        exception.Message.ShouldBe($"Usuário com ID {command.Id} não encontrado.");

        _repository.DidNotReceiveWithAnyArgs().Update(default!);
        await _repository.DidNotReceive().SaveChangesAsync();
    }

    [Fact]
    public async Task Dado_SenhaInvalida_Quando_AtualizarSenha_Entao_DeveLancarArgumentException()
    {
        var usuario = CriarUsuario();
        var command = new UpdatePasswordCommand(usuario.Id, "123");

        _repository.GetByIdAsync(command.Id).Returns(usuario);

        await Should.ThrowAsync<ArgumentException>(() => _sut.Handle(command));

        _repository.DidNotReceiveWithAnyArgs().Update(default!);
        await _repository.DidNotReceive().SaveChangesAsync();
    }

    private static Usuario CriarUsuario()
        => new("Nome", new DateTime(1990, 1, 1), Email.Create("user@email.com"), Senha.FromHash("Senha@123"), UserRole.User);
}
