using FgcGames.Application.Handlers;
using FgcGames.Domain.Entities;
using FgcGames.Domain.Interfaces.Repositories;
using FgcGames.Shared.Exceptions;
using NSubstitute;
using Shouldly;

namespace FgcGames.UnitTests.Application.Handlers;

public class GetTaskItemByIdExampleHandlerTests
{
    private readonly Microsoft.Extensions.Logging.ILogger<GetTaskItemByIdExampleHandler> _logger;
    private readonly ITaskItemExampleRepository _repository;
    private readonly GetTaskItemByIdExampleHandler _sut;

    public GetTaskItemByIdExampleHandlerTests()
    {
        _logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<GetTaskItemByIdExampleHandler>>();
        _repository = Substitute.For<ITaskItemExampleRepository>();

        _sut = new GetTaskItemByIdExampleHandler(_logger, _repository);
    }

    [Fact]
    public async Task Dado_TaskExiste_Quando_BuscarPorId_Entao_RetornaDadosDaTask()
    {
        // Arrange
        var id = 1;
        var task = new TaskItemExample("Minha Task");

        _repository.GetByIdAsync(id)
            .Returns(task);

        // Act
        var result = await _sut.Handle(id);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(task.Id);
        result.Title.ShouldBe(task.Title);
        result.IsCompleted.ShouldBe(task.IsCompleted);

        await _repository.Received(1).GetByIdAsync(id);
    }

    [Fact]
    public async Task Dado_TaskNaoExiste_Quando_BuscarPorId_Entao_LancaNotFoundException()
    {
        // Arrange
        var id = 1;

        _repository.GetByIdAsync(id)
            .Returns((TaskItemExample?)null);

        // Act
        var exception = await Should.ThrowAsync<NotFoundException>(() =>
            _sut.Handle(id)
        );

        // Assert
        exception.Message.ShouldBe("Task não encontrada");

        await _repository.Received(1).GetByIdAsync(id);
    }
}
