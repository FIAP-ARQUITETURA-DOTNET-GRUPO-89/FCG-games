using FgcGames.Application.Commands;
using FluentValidation;

namespace FgcGames.Application.Validators;

public class UpdateTaskItemExampleValidator : AbstractValidator<UpdateTaskItemExampleCommand>
{
    public UpdateTaskItemExampleValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(100);
    }
}
