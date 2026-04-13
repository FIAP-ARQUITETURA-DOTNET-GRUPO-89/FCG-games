using FgcGames.Application.Commands;
using FluentValidation;

namespace FgcGames.Application.Validators;

public class DeleteTaskItemExampleCommandValidator : AbstractValidator<DeleteTaskItemExampleCommand>
{
    public DeleteTaskItemExampleCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .LessThanOrEqualTo(int.MaxValue);
    }
}
