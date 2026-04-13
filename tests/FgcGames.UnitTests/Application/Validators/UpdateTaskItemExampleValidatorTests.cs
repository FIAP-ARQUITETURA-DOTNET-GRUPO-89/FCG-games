using FgcGames.Application.Commands;
using FgcGames.Application.Validators;
using FluentValidation.TestHelper;
using Shouldly;

namespace FgcGames.UnitTests.Application.Validators;

public class UpdateTaskItemExampleValidatorTests
{
    private readonly UpdateTaskItemExampleValidator _validator = new();

    [Fact]
    public void Dado_TituloValido_Quando_Validar_Entao_DeveSerValido()
    {
        // Arrange
        var command = new UpdateTaskItemExampleCommand("Novo Título", true);

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
        var command = new UpdateTaskItemExampleCommand(null!, false);

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
        var command = new UpdateTaskItemExampleCommand("", false);

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
        var command = new UpdateTaskItemExampleCommand("   ", false);

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
        var command = new UpdateTaskItemExampleCommand(titulo, false);

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
        var command = new UpdateTaskItemExampleCommand(titulo, false);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }
}
