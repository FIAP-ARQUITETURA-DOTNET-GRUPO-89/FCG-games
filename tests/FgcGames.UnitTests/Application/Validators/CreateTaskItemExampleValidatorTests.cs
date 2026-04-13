using FgcGames.Application.Commands;
using FgcGames.Application.Validators;
using FluentValidation.TestHelper;
using Shouldly;

namespace FgcGames.UnitTests.Application.Validators;

public class CreateTaskItemExampleValidatorTests
{
    private readonly CreateTaskItemExampleValidator _validator = new();

    [Fact]
    public void Dado_TituloValido_Quando_Validar_Entao_DeveSerValido()
    {
        // Arrange
        var command = new CreateTaskItemExampleCommand("Minha Task");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Dado_TituloNulo_Quando_Validar_Entao_DeveSerInvalido()
    {
        // Arrange
        var command = new CreateTaskItemExampleCommand(null!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Dado_TituloVazio_Quando_Validar_Entao_DeveSerInvalido()
    {
        // Arrange
        var command = new CreateTaskItemExampleCommand("");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Dado_TituloApenasComEspacos_Quando_Validar_Entao_DeveSerInvalido()
    {
        // Arrange
        var command = new CreateTaskItemExampleCommand("   ");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Dado_TituloCom100Caracteres_Quando_Validar_Entao_DeveSerValido()
    {
        // Arrange
        var titulo = new string('a', 100);
        var command = new CreateTaskItemExampleCommand(titulo);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Dado_TituloMaiorQue100Caracteres_Quando_Validar_Entao_DeveSerInvalido()
    {
        // Arrange
        var titulo = new string('a', 101);
        var command = new CreateTaskItemExampleCommand(titulo);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }
}
