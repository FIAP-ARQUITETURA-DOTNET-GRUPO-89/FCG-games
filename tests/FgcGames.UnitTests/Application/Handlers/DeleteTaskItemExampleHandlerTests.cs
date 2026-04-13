using FgcGames.Application.Commands;
using FgcGames.Application.Handlers;
using FgcGames.Domain.Entities;
using FgcGames.Domain.Interfaces.Repositories;
using FgcGames.Shared.Exceptions;
using NSubstitute;
using Shouldly;

namespace FgcGames.UnitTests.Application.Handlers;

public class DeleteTaskItemExampleHandlerTests
{
    private readonly Microsoft.Extensions.Logging.ILogger<DeleteTaskItemExampleHandler> _logger;
    private readonly ITaskItemExampleRepository _repository;
    private readonly DeleteTaskItemExampleHandler _sut;

    public DeleteTaskItemExampleHandlerTests()
    {
        _logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<DeleteTaskItemExampleHandler>>();
        _repository = Substitute.For<ITaskItemExampleRepository>();

        _sut = new DeleteTaskItemExampleHandler(_logger, _repository);
    }

    [Fact]
    public async Task Dado_TaskExiste_Quando_DeletarTask_Entao_DeletaComSucesso()
    {
        // Arrange
        var command = new DeleteTaskItemExampleCommand(Id: 1);
        var task = new TaskItemExample("Minha Task");

        _repository.GetByIdAsync(command.Id)
            .Returns(task);

        TaskItemExample? capturedTask = null;

        _repository
            .When(r => r.Delete(Arg.Any<TaskItemExample>()))
            .Do(callInfo => capturedTask = callInfo.Arg<TaskItemExample>());

        // Act
        await _sut.Handle(command);

        // Assert
        capturedTask.ShouldNotBeNull();
        capturedTask.ShouldBe(task);

        await _repository.Received(1).GetByIdAsync(command.Id);
        _repository.Received(1).Delete(task);
        await _repository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Dado_TaskNaoExiste_Quando_DeletarTask_Entao_LancaNotFoundException()
    {
        // Arrange
        var command = new DeleteTaskItemExampleCommand(Id: 1);

        _repository.GetByIdAsync(command.Id)
            .Returns((TaskItemExample?)null);

        // Act
        var exception = await Should.ThrowAsync<NotFoundException>(() =>
            _sut.Handle(command)
        );

        // Assert
        exception.Message.ShouldBe("Task não encontrada");

        await _repository.Received(1).GetByIdAsync(command.Id);
        _repository.DidNotReceiveWithAnyArgs().Delete(default!);
        await _repository.DidNotReceive().SaveChangesAsync();
    }
}
