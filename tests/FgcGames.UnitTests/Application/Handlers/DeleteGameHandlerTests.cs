// FgcGames.Tests/Handlers/DeleteJogoHandlerTests.cs
using FgcGames.Application.Commands;
using FgcGames.Application.Handlers;
using FgcGames.Domain.Entities;
using FgcGames.Domain.Enum;
using FgcGames.Domain.Interfaces.Repositories;
using Moq;

namespace FgcGames.Tests.Handlers;

public class DeleteJogoHandlerTests
{
    private readonly Mock<IGameRepository> _repositoryMock = new();
    private readonly DeleteGameHandler _handler;

    public DeleteJogoHandlerTests()
    {
        _handler = new DeleteGameHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_DeveInativarJogo_QuandoJogoExiste()
    {
        var jogo = new Jogo(DateTime.UtcNow, "Jogo", "Desc", 10m, ClassificacaoEtaria.Livre);
        _repositoryMock.Setup(r => r.ObterPorIdAsync(jogo.Id)).ReturnsAsync(jogo);
        _repositoryMock.Setup(r => r.RemoverAsync(jogo)).Returns(Task.CompletedTask);

        await _handler.Handle(new DeleteGameCommand(jogo.Id));

        _repositoryMock.Verify(r => r.RemoverAsync(jogo), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveLancarExcecao_QuandoJogoNaoEncontrado()
    {
        _repositoryMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Jogo?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(new DeleteGameCommand(Guid.NewGuid()))
        );
    }
}