using FgcGames.Domain.Entities;
using FgcGames.Infra.Repositories;
using FgcGames.UnitTests.TestHelpers.Factories;
using Shouldly;

namespace FgcGames.UnitTests.Infra.Repositories;

public class TaskItemExampleRepositoryTests
{
    [Fact]
    public async Task Dado_TaskValida_Quando_Adicionar_Entao_DevePersistir()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateContext();
        var repository = new TaskItemExampleRepository(context);

        var task = new TaskItemExample("Minha Task");

        // Act
        repository.Add(task);
        await repository.SaveChangesAsync();

        // Assert
        var result = await repository.GetByIdAsync(task.Id);

        result.ShouldNotBeNull();
        result!.Title.ShouldBe("Minha Task");
    }

    [Fact]
    public async Task Dado_TaskExistente_Quando_Atualizar_Entao_DevePersistirAlteracoes()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateContext();
        var repository = new TaskItemExampleRepository(context);

        var task = new TaskItemExample("Antigo");
        repository.Add(task);
        await repository.SaveChangesAsync();

        // Act
        task.UpdateTitle("Novo");
        repository.Update(task);
        await repository.SaveChangesAsync();

        // Assert
        var result = await repository.GetByIdAsync(task.Id);

        result.ShouldNotBeNull();
        result!.Title.ShouldBe("Novo");
    }

    [Fact]
    public async Task Dado_TaskExistente_Quando_Deletar_Entao_DeveRemover()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateContext();
        var repository = new TaskItemExampleRepository(context);

        var task = new TaskItemExample("Minha Task");
        repository.Add(task);
        await repository.SaveChangesAsync();

        // Act
        repository.Delete(task);
        await repository.SaveChangesAsync();

        // Assert
        var result = await repository.GetByIdAsync(task.Id);

        result.ShouldBeNull();
    }

    [Fact]
    public async Task Dado_TituloExistente_Quando_VerificarExistencia_Entao_DeveRetornarTrue()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateContext();
        var repository = new TaskItemExampleRepository(context);

        repository.Add(new TaskItemExample("Minha Task"));
        await repository.SaveChangesAsync();

        // Act
        var exists = await repository.ExistsByTitleAsync("Minha Task");

        // Assert
        exists.ShouldBeTrue();
    }

    [Fact]
    public async Task Dado_TituloInexistente_Quando_VerificarExistencia_Entao_DeveRetornarFalse()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateContext();
        var repository = new TaskItemExampleRepository(context);

        // Act
        var exists = await repository.ExistsByTitleAsync("Outra Task");

        // Assert
        exists.ShouldBeFalse();
    }

    [Fact]
    public async Task Dado_MultiplasTasks_Quando_BuscarTodas_Entao_DeveRetornarLista()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateContext();
        var repository = new TaskItemExampleRepository(context);

        repository.Add(new TaskItemExample("Task 1"));
        repository.Add(new TaskItemExample("Task 2"));
        await repository.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        result.Count.ShouldBe(2);
    }
}
