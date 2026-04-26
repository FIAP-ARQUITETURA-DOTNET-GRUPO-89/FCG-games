using FgcGames.Application.Commands;
using FgcGames.Application.Validators;
using FluentValidation.TestHelper;
using Shouldly;

namespace FgcGames.UnitTests.Application.Validators;

public class UpdateUserValidatorTests
{
    private readonly UpdateUserValidator _validator = new();

    [Fact]
    public void Dado_ComandoValido_Quando_Validar_Entao_DeveSerValido()
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "Jonatas", new DateTime(1990, 1, 1));

        var result = _validator.TestValidate(command);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Dado_NomeVazio_Quando_Validar_Entao_DeveTerErro()
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "", new DateTime(1990, 1, 1));

        var result = _validator.TestValidate(command);

        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Nome);
    }

    [Fact]
    public void Dado_DataNascimentoFutura_Quando_Validar_Entao_DeveTerErro()
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "Jonatas", DateTime.Today.AddDays(1));

        var result = _validator.TestValidate(command);

        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.DataNascimento);
    }
}
