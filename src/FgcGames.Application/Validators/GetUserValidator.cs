using FgcGames.Application.Queries;
using FluentValidation;

namespace FgcGames.Application.Validators;

public class GetUserValidator : AbstractValidator<GetUserQuery>
{
    public GetUserValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .LessThanOrEqualTo(int.MaxValue);
    }
}
