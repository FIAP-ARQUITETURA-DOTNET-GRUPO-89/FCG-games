using FgcGames.Application.Queries;
using FluentValidation;

namespace FgcGames.Application.Validators;

public class GetUsersByNameQueryValidator : AbstractValidator<GetUsersByNameQuery>
{
    public GetUsersByNameQueryValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage("O nome não pode estar vazio.")
            .MaximumLength(100)
            .WithMessage("O nome deve ter no máximo 100 caracteres.");
    }
}
