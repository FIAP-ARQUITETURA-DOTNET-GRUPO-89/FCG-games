using FgcGames.Application.Commands;
using FgcGames.Application.Validators;
using FluentValidation.TestHelper;
using Shouldly;

namespace FgcGames.UnitTests.Application.Validators;

public class DeleteUserValidatorTests
{
    private readonly DeleteUserValidator _validator = new();

    [Fact]
    public void Dado_IdValido_Quando_Validar_Entao_DeveSerValido()
    {
        var command = new DeleteUserCommand(Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.IsValid.ShouldBeTrue();
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Dado_IdVazio_Quando_Validar_Entao_DeveSerInvalido()
    {
        var command = new DeleteUserCommand(Guid.Empty);

        var result = _validator.TestValidate(command);

        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
