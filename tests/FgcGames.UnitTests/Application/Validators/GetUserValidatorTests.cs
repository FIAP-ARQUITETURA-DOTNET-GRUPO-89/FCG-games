using FgcGames.Application.Queries;
using FgcGames.Application.Validators;
using FluentValidation.TestHelper;
using Shouldly;

namespace FgcGames.UnitTests.Application.Validators;

public class GetUserValidatorTests
{
    private readonly GetUserValidator _validator = new();

    [Fact]
    public void Dado_IdValido_Quando_Validar_Entao_DeveSerValido()
    {
        var query = new GetUserQuery(1);

        var result = _validator.TestValidate(query);

        result.IsValid.ShouldBeTrue();
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Dado_IdIgualAZero_Quando_Validar_Entao_DeveSerInvalido()
    {
        var query = new GetUserQuery(0);

        var result = _validator.TestValidate(query);

        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Dado_IdNegativo_Quando_Validar_Entao_DeveSerInvalido()
    {
        var query = new GetUserQuery(-1);

        var result = _validator.TestValidate(query);

        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
