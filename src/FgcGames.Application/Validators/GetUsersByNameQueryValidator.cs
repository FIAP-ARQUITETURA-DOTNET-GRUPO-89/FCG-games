using FgcGames.Application.Queries;
using FluentValidation;

namespace FgcGames.Application.Validators;

public class GetUsersByNameQueryValidator : AbstractValidator<GetUsersByNameQuery>
{
    public GetUsersByNameQueryValidator()
    {
        RuleFor(x => x.Nome)
            .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Pagina)
            .GreaterThan(0).WithMessage("A página deve ser maior que 0.");

        RuleFor(x => x.TamanhoPagina)
            .InclusiveBetween(1, 100).WithMessage("O tamanho da página deve ser entre 1 e 100.");
    }
}
