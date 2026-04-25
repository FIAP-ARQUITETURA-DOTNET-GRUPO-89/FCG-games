using FgcGames.Domain.ValueObjects;
using Shouldly;

namespace FgcGames.UnitTests.Domain.ValueObjects;

public class SenhaTests
{
    [Fact]
    public void Dado_HashValido_Quando_Criar_Entao_DeveCriarSenha()
    {
        // Arrange
        var hash = "hash_seguro_123";

        // Act
        var senha = Senha.FromHash(hash);

        // Assert
        senha.ShouldNotBeNull();
        senha.Hash.ShouldBe(hash);
    }

    [Fact]
    public void Dado_HashNulo_Quando_Criar_Entao_DeveLancarExcecao()
    {
        // Arrange
        string hash = null;

        // Act & Assert
        Should.Throw<ArgumentException>(() => Senha.FromHash(hash))
              .Message.ShouldBe("O hash da senha não pode ser vazio.");
    }

    [Fact]
    public void Dado_HashVazio_Quando_Criar_Entao_DeveLancarExcecao()
    {
        // Arrange
        var hash = "";

        // Act & Assert
        Should.Throw<ArgumentException>(() => Senha.FromHash(hash))
              .Message.ShouldBe("O hash da senha não pode ser vazio.");
    }

    [Fact]
    public void Dado_HashComEspacos_Quando_Criar_Entao_DeveLancarExcecao()
    {
        // Arrange
        var hash = "   ";

        // Act & Assert
        Should.Throw<ArgumentException>(() => Senha.FromHash(hash))
              .Message.ShouldBe("O hash da senha não pode ser vazio.");
    }

    [Theory]
    [InlineData("a")]
    [InlineData("123")]
    [InlineData("!@#hash")]
    [InlineData("HASH_GRANDE_123456789")]
    public void Dado_HashValido_Quando_Criar_Entao_DeveAceitarDiferentesFormatos(string hash)
    {
        // Arrange & Act
        var senha = Senha.FromHash(hash);

        // Assert
        senha.Hash.ShouldBe(hash);
    }
}
