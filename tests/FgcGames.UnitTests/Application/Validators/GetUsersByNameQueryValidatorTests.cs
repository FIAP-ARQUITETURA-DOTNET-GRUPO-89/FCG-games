using FgcGames.Application.Queries;
using FgcGames.Application.Validators;
using FluentValidation.TestHelper;
using Shouldly;

namespace FgcGames.UnitTests.Application.Validators;

public class GetUsersByNameQueryValidatorTests
{
    private readonly GetUsersByNameQueryValidator _validator = new();

    [Fact]
    public void Dado_NomeValido_Quando_Validar_Entao_DeveSerValido()
    {
        var query = new GetUsersByNameQuery("Jon", 1, 10);

        var result = _validator.TestValidate(query);

        result.IsValid.ShouldBeTrue();
        result.ShouldNotHaveValidationErrorFor(x => x.Nome);
    }

    [Fact]
    public void Dado_NomeVazio_Quando_Validar_Entao_DeveSerInvalido()
    {
        var query = new GetUsersByNameQuery("", 1, 10);

        var result = _validator.TestValidate(query);

        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Nome);
    }

    [Fact]
    public void Dado_NomeMaiorQue100Caracteres_Quando_Validar_Entao_DeveSerInvalido()
    {
        var nome = new string('a', 101);
        var query = new GetUsersByNameQuery(nome, 1, 10);

        var result = _validator.TestValidate(query);

        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Nome);
    }
}
