using FgcGames.Application.Commands;
using FgcGames.Application.Validators;
using FluentValidation.TestHelper;
using Shouldly;

namespace FgcGames.UnitTests.Application.Validators;

public class LoginValidatorTests
{
    private readonly LoginValidator _validator = new();

    [Fact]
    public void Dado_EmailESenhaValidos_Quando_Validar_Entao_DeveSerValido()
    {
        // Arrange
        var command = new LoginCommand("teste@email.com", "Senha12@");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
        result.ShouldNotHaveValidationErrorFor(x => x.Senha);
    }

    [Fact]
    public void Dado_EmailNulo_Quando_Validar_Entao_DeveSerInvalido()
    {
        // Arrange
        var command = new LoginCommand(null!, "Senha12@");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Dado_EmailVazio_Quando_Validar_Entao_DeveSerInvalido()
    {
        // Arrange
        var command = new LoginCommand("", "Senha12@");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Dado_EmailMaiorQue254Caracteres_Quando_Validar_Entao_DeveSerInvalido()
    {
        // Arrange
        var email = new string('a', 255) + "@email.com";
        var command = new LoginCommand(email, "Senha12@");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Dado_EmailInvalido_Quando_Validar_Entao_DeveSerInvalido()
    {
        // Arrange
        var command = new LoginCommand("emailinvalido", "Senha12@");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Dado_SenhaVazia_Quando_Validar_Entao_DeveSerInvalida()
    {
        // Arrange
        var command = new LoginCommand("teste@email.com", "");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Senha);
    }

    [Fact]
    public void Dado_SenhaNula_Quando_Validar_Entao_DeveSerInvalida()
    {
        // Arrange
        var command = new LoginCommand("teste@email.com", null!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Senha);
    }

    [Fact]
    public void Dado_SenhaCom7Caracteres_Quando_Validar_Entao_DeveSerValida()
    {
        // Arrange
        var command = new LoginCommand("teste@email.com", "Senha1@");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Senha);
    }

    [Fact]
    public void Dado_SenhaCom12Caracteres_Quando_Validar_Entao_DeveSerValida()
    {
        // Arrange
        var command = new LoginCommand("teste@email.com", "Senha1234@#$");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.ShouldNotHaveValidationErrorFor(x => x.Senha);
    }

    [Fact]
    public void Dado_SenhaCom13Caracteres_Quando_Validar_Entao_DeveSerInvalida()
    {
        // Arrange
        var command = new LoginCommand("teste@email.com", "Senha1234@#$1");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Senha);
    }

    [Fact]
    public void Dado_SenhaSemLetraMaiuscula_Quando_Validar_Entao_DeveSerInvalida()
    {
        // Arrange
        var command = new LoginCommand("teste@email.com", "senha12@");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Senha);
    }

    [Fact]
    public void Dado_SenhaSemLetraMinuscula_Quando_Validar_Entao_DeveSerInvalida()
    {
        // Arrange
        var command = new LoginCommand("teste@email.com", "SENHA12@");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Senha);
    }

    [Fact]
    public void Dado_SenhaSemNumero_Quando_Validar_Entao_DeveSerInvalida()
    {
        // Arrange
        var command = new LoginCommand("teste@email.com", "Senha@@@");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Senha);
    }

    [Fact]
    public void Dado_SenhaSemCaractereEspecial_Quando_Validar_Entao_DeveSerInvalida()
    {
        // Arrange
        var command = new LoginCommand("teste@email.com", "Senha1234");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Senha);
    }

    [Theory]
    [InlineData("Senha12!")]
    [InlineData("Senha12?")]
    [InlineData("Senha12*")]
    [InlineData("Senha12.")]
    [InlineData("Senha12@")]
    [InlineData("Senha12#")]
    [InlineData("Senha12$")]
    [InlineData("Senha12%")]
    [InlineData("Senha12&")]
    public void Dado_SenhaComCaracterEspecialValido_Quando_Validar_Entao_DeveSerValida(string senha)
    {
        // Arrange
        var command = new LoginCommand("teste@email.com", senha);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.ShouldNotHaveValidationErrorFor(x => x.Senha);
    }
}
