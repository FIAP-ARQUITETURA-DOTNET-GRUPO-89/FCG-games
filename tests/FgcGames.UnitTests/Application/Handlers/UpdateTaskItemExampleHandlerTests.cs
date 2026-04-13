using FgcGames.Application.Commands;
using FgcGames.Application.Handlers;
using FgcGames.Domain.Entities;
using FgcGames.Domain.Interfaces.Repositories;
using FgcGames.Shared.Exceptions;
using NSubstitute;
using Shouldly;

namespace FgcGames.UnitTests.Application.Handlers;

public class UpdateTaskItemExampleHandlerTests
{
    private readonly Microsoft.Extensions.Logging.ILogger<UpdateTaskItemExampleHandler> _logger;
    private readonly ITaskItemExampleRepository _repository;
    private readonly UpdateTaskItemExampleHandler _sut;

    public UpdateTaskItemExampleHandlerTests()
    {
        _logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<UpdateTaskItemExampleHandler>>();
        _repository = Substitute.For<ITaskItemExampleRepository>();

        _sut = new UpdateTaskItemExampleHandler(_logger, _repository);
    }

    [Fact]
    public async Task Dado_TaskExiste_Quando_Atualizar_Entao_AtualizaComSucesso()
    {
        // Arrange
        var id = 1;
        var command = new UpdateTaskItemExampleCommand("Novo Título", true);
        var task = new TaskItemExample("Título Antigo");

        _repository.GetByIdAsync(id).Returns(task);
        _repository.ExistsByTitleAsync(command.Title).Returns(false);

        TaskItemExample? capturedTask = null;

        _repository
            .When(r => r.Update(Arg.Any<TaskItemExample>()))
            .Do(callInfo => capturedTask = callInfo.Arg<TaskItemExample>());

        // Act
        await _sut.Handle(id, command);

        // Assert
        capturedTask.ShouldNotBeNull();
        capturedTask.Title.ShouldBe(command.Title);
        capturedTask.IsCompleted.ShouldBeTrue();

        await _repository.Received(1).GetByIdAsync(id);
        await _repository.Received(1).ExistsByTitleAsync(command.Title);
        _repository.Received(1).Update(task);
        await _repository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Dado_TaskNaoExiste_Quando_Atualizar_Entao_LancaNotFoundException()
    {
        // Arrange
        var id = 1;
        var command = new UpdateTaskItemExampleCommand("Novo Título", true);

        _repository.GetByIdAsync(id).Returns((TaskItemExample?)null);

        // Act
        var exception = await Should.ThrowAsync<NotFoundException>(() =>
            _sut.Handle(id, command)
        );

        // Assert
        exception.Message.ShouldBe("Task não encontrada");

        await _repository.Received(1).GetByIdAsync(id);
        _repository.DidNotReceiveWithAnyArgs().Update(default!);
        await _repository.DidNotReceive().SaveChangesAsync();
    }

    [Fact]
    public async Task Dado_TituloJaExisteEEDiferente_Quando_Atualizar_Entao_LancaAlreadyExistsException()
    {
        // Arrange
        var id = 1;
        var command = new UpdateTaskItemExampleCommand("Novo Título", false);
        var task = new TaskItemExample("Título Antigo");

        _repository.GetByIdAsync(id).Returns(task);
        _repository.ExistsByTitleAsync(command.Title).Returns(true);

        // Act
        var exception = await Should.ThrowAsync<AlreadyExistsException>(() =>
            _sut.Handle(id, command)
        );

        // Assert
        exception.Message.ShouldBe("Já existe uma task com esse título");

        await _repository.Received(1).GetByIdAsync(id);
        await _repository.Received(1).ExistsByTitleAsync(command.Title);
        _repository.DidNotReceiveWithAnyArgs().Update(default!);
        await _repository.DidNotReceive().SaveChangesAsync();
    }

    [Fact]
    public async Task Dado_MesmoTitulo_Quando_Atualizar_Entao_PermiteAtualizacao()
    {
        // Arrange
        var id = 1;
        var command = new UpdateTaskItemExampleCommand("Mesmo Título", false);
        var task = new TaskItemExample("Mesmo Título");

        _repository.GetByIdAsync(id).Returns(task);
        _repository.ExistsByTitleAsync(command.Title).Returns(true);

        // Act
        await _sut.Handle(id, command);

        // Assert
        task.Title.ShouldBe(command.Title);

        await _repository.Received(1).GetByIdAsync(id);
        await _repository.Received(1).ExistsByTitleAsync(command.Title);
        _repository.Received(1).Update(task);
        await _repository.Received(1).SaveChangesAsync();
    }
}
