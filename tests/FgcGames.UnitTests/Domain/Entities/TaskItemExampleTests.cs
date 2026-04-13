using FgcGames.Domain.Entities;
using Shouldly;

namespace FgcGames.UnitTests.Domain.Entities;

public class TaskItemExampleTests
{
    [Fact]
    public void Dado_TituloValido_Quando_CriarTask_Entao_DeveInicializarCorretamente()
    {
        // Arrange
        var title = "Minha Task";

        // Act
        var task = new TaskItemExample(title);

        // Assert
        task.ShouldNotBeNull();
        task.Title.ShouldBe(title);
        task.IsCompleted.ShouldBeFalse();
    }

    [Fact]
    public void Dado_TituloNulo_Quando_CriarTask_Entao_DeveLancarException()
    {
        // Arrange
        string? title = null;

        // Act
        var exception = Should.Throw<ArgumentException>(() => new TaskItemExample(title!));

        // Assert
        exception.Message.ShouldBe("Title inválido");
    }

    [Fact]
    public void Dado_TituloVazio_Quando_CriarTask_Entao_DeveLancarException()
    {
        // Arrange
        var title = "";

        // Act
        var exception = Should.Throw<ArgumentException>(() => new TaskItemExample(title));

        // Assert
        exception.Message.ShouldBe("Title inválido");
    }

    [Fact]
    public void Dado_TituloApenasComEspacos_Quando_CriarTask_Entao_DeveLancarException()
    {
        // Arrange
        var title = "   ";

        // Act
        var exception = Should.Throw<ArgumentException>(() => new TaskItemExample(title));

        // Assert
        exception.Message.ShouldBe("Title inválido");
    }

    [Fact]
    public void Dado_TituloMaiorQue100Caracteres_Quando_CriarTask_Entao_DeveLancarException()
    {
        // Arrange
        var title = new string('a', 101);

        // Act
        var exception = Should.Throw<ArgumentException>(() => new TaskItemExample(title));

        // Assert
        exception.Message.ShouldBe("Title deve ter no máximo 100 caracteres");
    }

    [Fact]
    public void Dado_TaskNaoConcluida_Quando_Completar_Entao_DeveMarcarComoConcluida()
    {
        // Arrange
        var task = new TaskItemExample("Minha Task");

        // Act
        task.Complete();

        // Assert
        task.IsCompleted.ShouldBeTrue();
    }

    [Fact]
    public void Dado_TaskJaConcluida_Quando_CompletarNovamente_Entao_DevePermanecerConcluida()
    {
        // Arrange
        var task = new TaskItemExample("Minha Task");
        task.Complete();

        // Act
        task.Complete();

        // Assert
        task.IsCompleted.ShouldBeTrue();
    }

    [Fact]
    public void Dado_NovoTituloValido_Quando_AtualizarTitulo_Entao_DeveAlterarTitulo()
    {
        // Arrange
        var task = new TaskItemExample("Título Antigo");
        var novoTitulo = "Título Novo";

        // Act
        task.UpdateTitle(novoTitulo);

        // Assert
        task.Title.ShouldBe(novoTitulo);
    }

    [Fact]
    public void Dado_TituloNulo_Quando_AtualizarTitulo_Entao_DeveLancarException()
    {
        // Arrange
        var task = new TaskItemExample("Título Antigo");

        // Act
        var exception = Should.Throw<ArgumentException>(() => task.UpdateTitle(null!));

        // Assert
        exception.Message.ShouldBe("Title inválido");
    }

    [Fact]
    public void Dado_TituloVazio_Quando_AtualizarTitulo_Entao_DeveLancarException()
    {
        // Arrange
        var task = new TaskItemExample("Título Antigo");

        // Act
        var exception = Should.Throw<ArgumentException>(() => task.UpdateTitle(""));

        // Assert
        exception.Message.ShouldBe("Title inválido");
    }

    [Fact]
    public void Dado_TituloApenasComEspacos_Quando_AtualizarTitulo_Entao_DeveLancarException()
    {
        // Arrange
        var task = new TaskItemExample("Título Antigo");

        // Act
        var exception = Should.Throw<ArgumentException>(() => task.UpdateTitle("   "));

        // Assert
        exception.Message.ShouldBe("Title inválido");
    }

    [Fact]
    public void Dado_TituloMaiorQue100Caracteres_Quando_AtualizarTitulo_Entao_DeveLancarException()
    {
        // Arrange
        var task = new TaskItemExample("Título Antigo");
        var titulo = new string('a', 101);

        // Act
        var exception = Should.Throw<ArgumentException>(() => task.UpdateTitle(titulo));

        // Assert
        exception.Message.ShouldBe("Title deve ter no máximo 100 caracteres");
    }
}
