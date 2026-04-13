using FgcGames.Application.Queries;
using FluentValidation;

namespace FgcGames.Application.Validators;

public class GetTaskItemByIdExampleValidator : AbstractValidator<GetTaskItemByIdExampleQuery>
{
    public GetTaskItemByIdExampleValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .LessThanOrEqualTo(int.MaxValue);
    }
}
