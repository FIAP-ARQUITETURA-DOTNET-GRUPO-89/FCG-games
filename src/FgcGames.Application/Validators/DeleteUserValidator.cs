using FgcGames.Application.Commands;
using FluentValidation;

namespace FgcGames.Application.Validators;

public class DeleteUserValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .LessThanOrEqualTo(int.MaxValue);
    }
}
