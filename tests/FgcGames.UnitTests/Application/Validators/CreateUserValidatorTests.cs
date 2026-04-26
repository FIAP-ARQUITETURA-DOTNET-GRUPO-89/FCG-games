using FgcGames.Application.Commands;
using FluentValidation.TestHelper;
using Shouldly;

namespace FgcGames.UnitTests.Application.Validators;

public class CreateUserValidatorTests
{
    private readonly CreateUserValidator _validator = new();

    [Fact]
    public void Dado_ComandoValido_Quando_Validar_Entao_DeveSerValido()
    {
        var command = new CreateUserCommand("Jonatas", "jonatas@email.com", new DateTime(1990, 1, 1), "Senha*123");

        var result = _validator.TestValidate(command);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Dado_NomeInvalido_Quando_Validar_Entao_DeveTerErro()
    {
        var command = new CreateUserCommand("", "jonatas@email.com", new DateTime(1990, 1, 1), "Senha@123");

        var result = _validator.TestValidate(command);

        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Nome);
    }

    [Fact]
    public void Dado_EmailInvalido_Quando_Validar_Entao_DeveTerErro()
    {
        var command = new CreateUserCommand("Jonatas", "email-invalido", new DateTime(1990, 1, 1), "Senha@123");

        var result = _validator.TestValidate(command);

        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Dado_SenhaFraca_Quando_Validar_Entao_DeveTerErro()
    {
        var command = new CreateUserCommand("Jonatas", "jonatas@email.com", new DateTime(1990, 1, 1), "12345678");

        var result = _validator.TestValidate(command);

        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Senha);
    }
}
