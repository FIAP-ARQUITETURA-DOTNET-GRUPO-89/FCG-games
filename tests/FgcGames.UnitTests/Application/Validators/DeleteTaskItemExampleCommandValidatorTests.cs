using FgcGames.Application.Commands;
using FgcGames.Application.Validators;
using FluentValidation.TestHelper;
using Shouldly;

namespace FgcGames.UnitTests.Application.Validators;

public class DeleteTaskItemExampleCommandValidatorTests
{
    private readonly DeleteTaskItemExampleCommandValidator _validator = new();

    [Fact]
    public void Dado_IdValido_Quando_Validar_Entao_DeveSerValido()
    {
        // Arrange
        var command = new DeleteTaskItemExampleCommand(1);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Dado_IdIgualAZero_Quando_Validar_Entao_DeveSerInvalido()
    {
        // Arrange
        var command = new DeleteTaskItemExampleCommand(0);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Dado_IdNegativo_Quando_Validar_Entao_DeveSerInvalido()
    {
        // Arrange
        var command = new DeleteTaskItemExampleCommand(-1);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
