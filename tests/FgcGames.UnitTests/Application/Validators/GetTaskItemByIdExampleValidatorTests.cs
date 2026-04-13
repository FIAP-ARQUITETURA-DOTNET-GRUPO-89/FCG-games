using FgcGames.Application.Queries;
using FgcGames.Application.Validators;
using FluentValidation.TestHelper;
using Shouldly;

namespace FgcGames.UnitTests.Application.Validators;

public class GetTaskItemByIdExampleValidatorTests
{
    private readonly GetTaskItemByIdExampleValidator _validator = new();

    [Fact]
    public void Dado_IdValido_Quando_Validar_Entao_DeveSerValido()
    {
        // Arrange
        var query = new GetTaskItemByIdExampleQuery(1);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Dado_IdIgualAZero_Quando_Validar_Entao_DeveSerInvalido()
    {
        // Arrange
        var query = new GetTaskItemByIdExampleQuery(0);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Dado_IdNegativo_Quando_Validar_Entao_DeveSerInvalido()
    {
        // Arrange
        var query = new GetTaskItemByIdExampleQuery(-1);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
