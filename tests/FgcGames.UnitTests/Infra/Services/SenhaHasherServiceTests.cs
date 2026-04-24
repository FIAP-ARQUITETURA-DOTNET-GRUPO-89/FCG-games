using FgcGames.Infra.Services;
using Shouldly;

namespace FgcGames.UnitTests.Infra.Services;

public class SenhaHasherServiceTests
{
    private readonly SenhaHasherService _sut = new();

    [Fact]
    public void Dado_SenhaValida_Quando_Hash_Entao_RetornaHashDiferenteDaSenhaOriginal()
    {
        // Arrange
        var senha = "Senha@123";

        // Act
        var hash = _sut.Hash(senha);

        // Assert
        hash.ShouldNotBeNullOrWhiteSpace();
        hash.ShouldNotBe(senha);
    }

    [Fact]
    public void Dado_SenhaEHash_Quando_VerificarSenhaCorreta_Entao_RetornaTrue()
    {
        // Arrange
        var senha = "Senha@123";
        var hash = _sut.Hash(senha);

        // Act
        var resultado = _sut.VerificarSenha(senha, hash);

        // Assert
        resultado.ShouldBeTrue();
    }

    [Fact]
    public void Dado_SenhaIncorreta_Quando_Verificar_Entao_RetornaFalse()
    {
        // Arrange
        var senha = "Senha@123";
        var senhaErrada = "SenhaErrada@123";

        var hash = _sut.Hash(senha);

        // Act
        var resultado = _sut.VerificarSenha(senhaErrada, hash);

        // Assert
        resultado.ShouldBeFalse();
    }

    [Fact]
    public void Dado_MesmaSenha_Quando_GerarHashDuasVezes_Entao_ResultadoDeveSerDiferente()
    {
        // Arrange
        var senha = "Senha@123";

        // Act
        var hash1 = _sut.Hash(senha);
        var hash2 = _sut.Hash(senha);

        // Assert
        hash1.ShouldNotBe(hash2);
    }

    [Fact]
    public void Dado_HashGerado_Quando_VerificarMultiplasVezes_Entao_SempreRetornaTrue()
    {
        // Arrange
        var senha = "Senha@123";
        var hash = _sut.Hash(senha);

        // Act & Assert
        _sut.VerificarSenha(senha, hash).ShouldBeTrue();
        _sut.VerificarSenha(senha, hash).ShouldBeTrue();
        _sut.VerificarSenha(senha, hash).ShouldBeTrue();
    }

    [Fact]
    public void Dado_SenhaVazia_Quando_Hash_Entao_GeraHashValido()
    {
        // Arrange
        var senha = "";

        // Act
        var hash = _sut.Hash(senha);

        // Assert
        hash.ShouldNotBeNullOrWhiteSpace();
        _sut.VerificarSenha(senha, hash).ShouldBeTrue();
    }

    [Fact]
    public void Dado_HashInvalido_Quando_Verificar_Entao_RetornaFalse()
    {
        // Arrange
        var senha = "Senha@123";
        var hashInvalido = "hash-invalido";

        // Act
        var resultado = _sut.VerificarSenha(senha, hashInvalido);

        // Assert
        resultado.ShouldBeFalse();
    }
}
