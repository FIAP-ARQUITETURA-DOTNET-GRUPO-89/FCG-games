using FgcGames.Application.Commands;
using FluentValidation;

namespace FgcGames.Application.Validators;

public class CreateTaskItemExampleValidator : AbstractValidator<CreateTaskItemExampleCommand>
{
    public CreateTaskItemExampleValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(100);
    }
}
