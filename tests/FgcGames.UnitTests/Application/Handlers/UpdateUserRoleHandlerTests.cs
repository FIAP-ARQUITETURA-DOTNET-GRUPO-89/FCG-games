using FgcGames.Application.Commands;
using FgcGames.Application.Handlers;
using FgcGames.Domain.Entities;
using FgcGames.Domain.Enum;
using FgcGames.Domain.Interfaces.Repositories;
using FgcGames.Domain.ValueObjects;
using NSubstitute;
using Shouldly;

namespace FgcGames.UnitTests.Application.Handlers;

public class UpdateUserRoleHandlerTests
{
    private readonly Microsoft.Extensions.Logging.ILogger<UpdateUserRoleHandler> _logger;
    private readonly IUsuarioRepository _repository;
    private readonly UpdateUserRoleHandler _sut;

    public UpdateUserRoleHandlerTests()
    {
        _logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<UpdateUserRoleHandler>>();
        _repository = Substitute.For<IUsuarioRepository>();
        _sut = new UpdateUserRoleHandler(_logger, _repository);
    }

    [Fact]
    public async Task Dado_UsuarioExistente_Quando_AtualizarRole_Entao_DeveAtualizarComSucesso()
    {
        var usuario = CriarUsuario();
        var command = new UpdateUserRoleCommand(usuario.Id, UserRole.Admin);

        _repository.GetByIdAsync(command.Id).Returns(usuario);

        var result = await _sut.Handle(command);

        result.Id.ShouldBe(usuario.Id);
        result.Role.ShouldBe(UserRole.Admin);
        result.Mensagem.ShouldBe("Role atualizada com sucesso!");

        usuario.Role.ShouldBe(UserRole.Admin);

        await _repository.Received(1).GetByIdAsync(command.Id);
        _repository.Received(1).Update(usuario);
        await _repository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Dado_UsuarioInexistente_Quando_AtualizarRole_Entao_DeveLancarException()
    {
        var command = new UpdateUserRoleCommand(Guid.NewGuid(), UserRole.Admin);

        _repository.GetByIdAsync(command.Id).Returns((Usuario?)null);

        var exception = await Should.ThrowAsync<Exception>(() => _sut.Handle(command));

        exception.Message.ShouldBe($"Usuário com ID {command.Id} não encontrado.");

        await _repository.Received(1).GetByIdAsync(command.Id);
        _repository.DidNotReceiveWithAnyArgs().Update(default!);
        await _repository.DidNotReceive().SaveChangesAsync();
    }

    private static Usuario CriarUsuario()
        => new("Nome", new DateOnly(1990, 1, 1), Email.Create("user@email.com"), Senha.FromHash("Senha@123"), UserRole.User);
}
