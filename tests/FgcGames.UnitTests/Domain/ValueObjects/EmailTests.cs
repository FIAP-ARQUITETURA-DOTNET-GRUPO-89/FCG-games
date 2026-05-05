using FgcGames.Domain.ValueObjects;
using Shouldly;

namespace FgcGames.UnitTests.Domain.ValueObjects;

public class EmailTests
{
    [Fact]
    public void Dado_EmailValido_Quando_Criar_Entao_DeveNormalizarParaMinusculoETrim()
    {
        // Arrange
        var endereco = "  TESTE@EMAIL.COM  ";

        // Act
        var email = Email.Create(endereco);

        // Assert
        email.Endereco.ShouldBe("teste@email.com");
    }

    [Fact]
    public void Dado_EmailValido_Quando_Criar_Entao_DeveManterValorCorreto()
    {
        // Arrange
        var endereco = "user@fgcgames.com";

        // Act
        var email = Email.Create(endereco);

        // Assert
        email.Endereco.ShouldBe(endereco);
    }

    [Fact]
    public void Dado_EmailNulo_Quando_Criar_Entao_DeveLancarExcecao()
    {
        // Arrange
        string? endereco = null;

        // Act & Assert
        Should.Throw<ArgumentException>(() => Email.Create(endereco!))
                .Message.ShouldBe("E-mail inválido.");
    }

    [Fact]
    public void Dado_EmailVazio_Quando_Criar_Entao_DeveLancarExcecao()
    {
        // Arrange
        var endereco = "";

        // Act & Assert
        Should.Throw<ArgumentException>(() => Email.Create(endereco))
                .Message.ShouldBe("E-mail inválido.");
    }

    [Fact]
    public void Dado_EmailSemArroba_Quando_Criar_Entao_DeveLancarExcecao()
    {
        // Arrange
        var endereco = "emailinvalido";

        // Act & Assert
        Should.Throw<ArgumentException>(() => Email.Create(endereco))
                .Message.ShouldBe("E-mail inválido.");
    }

    [Fact]
    public void Dado_EmailComEspacos_Quando_Criar_Entao_DeveRemoverEspacos()
    {
        // Arrange
        var endereco = "  user@email.com  ";

        // Act
        var email = Email.Create(endereco);

        // Assert
        email.Endereco.ShouldBe("user@email.com");
    }

    [Theory]
    [InlineData("TESTE@EMAIL.COM", "teste@email.com")]
    [InlineData("User@Domain.COM", "user@domain.com")]
    [InlineData("ADMIN@FGCGAMES.COM", "admin@fgcgames.com")]
    public void Dado_EmailComLetrasMaiusculas_Quando_Criar_Entao_DeveConverterParaMinusculo(string entrada, string esperado)
    {
        // Arrange & Act
        var email = Email.Create(entrada);

        // Assert
        email.Endereco.ShouldBe(esperado);
    }
}
