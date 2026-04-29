using FgcGames.Application.Commands;
using FgcGames.Application.Handlers;
using FgcGames.Domain.Entities;
using FgcGames.Domain.Enum;
using FgcGames.Domain.Interfaces.Repositories;
using Moq;

namespace FgcGames.Tests.Handlers;

public class CreateJogoHandlerTests
{
    private readonly Mock<IGameRepository> _repositoryMock = new();
    private readonly CreateGameHandler _handler;

    public CreateJogoHandlerTests()
    {
        _handler = new CreateGameHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarJogoResponse_QuandoComandoValido()
    {
        var command = new CreateGameCommand(
            "Meu jogo favorito",
            "Descrição do jogo",
            499,
            DateTime.UtcNow.AddMonths(1),
            ClassificacaoEtaria.Livre
        );

        _repositoryMock
            .Setup(r => r.AdicionarAsync(It.IsAny<Jogo>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command);

        Assert.NotNull(result);
        Assert.Equal("Meu jogo favorito", result.Nome);
        Assert.Equal(499m, result.Preco);
        _repositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Jogo>()), Times.Once);
    }
}