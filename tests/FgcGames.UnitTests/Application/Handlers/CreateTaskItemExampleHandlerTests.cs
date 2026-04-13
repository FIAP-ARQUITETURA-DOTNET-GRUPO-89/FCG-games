using FgcGames.Application.Commands;
using FgcGames.Application.Handlers;
using FgcGames.Domain.Entities;
using FgcGames.Domain.Interfaces.Repositories;
using FgcGames.Shared.Exceptions;
using NSubstitute;
using Shouldly;

namespace FgcGames.UnitTests.Application.Handlers;

public class CreateTaskItemExampleHandlerTests
{
    private readonly Microsoft.Extensions.Logging.ILogger<CreateTaskItemExampleHandler> _logger;
    private readonly ITaskItemExampleRepository _repository;
    private readonly CreateTaskItemExampleHandler _sut;

    public CreateTaskItemExampleHandlerTests()
    {
        _logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<CreateTaskItemExampleHandler>>();
        _repository = Substitute.For<ITaskItemExampleRepository>();

        _sut = new CreateTaskItemExampleHandler(_logger, _repository);
    }

    [Fact]
    public async Task Dado_TaskNaoExiste_Quando_CriarTask_Entao_CriaComSucesso()
    {
        // Arrange
        var command = new CreateTaskItemExampleCommand("Minha Task");

        _repository.ExistsByTitleAsync(command.Title)
            .Returns(false);

        TaskItemExample? capturedTask = null;

		_repository
			.When(r => r.Add(Arg.Any<TaskItemExample>()))
			.Do(callInfo => capturedTask = callInfo.Arg<TaskItemExample>());

		// Act
		var result = await _sut.Handle(command);

        // Assert
        result.ShouldNotBeNull();
        result.Title.ShouldBe(command.Title);
        result.IsCompleted.ShouldBeFalse();

        capturedTask.ShouldNotBeNull();
		capturedTask.Title.ShouldBe(command.Title);
		capturedTask.IsCompleted.ShouldBeFalse();

		await _repository.Received(1).ExistsByTitleAsync(command.Title);
        _repository.Received(1).Add(capturedTask);
        await _repository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Dado_TaskJaExiste_Quando_CriarTask_Entao_LancaAlreadyExistsException()
    {
        // Arrange
        var command = new CreateTaskItemExampleCommand("Minha Task");

        _repository.ExistsByTitleAsync(command.Title)
            .Returns(true);

        // Act
        var exception = await Should.ThrowAsync<AlreadyExistsException>(() =>
            _sut.Handle(command)
        );

        // Assert
        exception.Message.ShouldBe("Já existe uma task com esse título");

		await _repository.Received(1).ExistsByTitleAsync(command.Title);
		_repository.DidNotReceiveWithAnyArgs().Add(default!);
		await _repository.DidNotReceive().SaveChangesAsync();
    }
}
