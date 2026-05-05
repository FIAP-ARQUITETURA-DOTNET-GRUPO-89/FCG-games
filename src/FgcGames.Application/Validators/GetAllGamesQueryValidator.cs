using FluentValidation;
using FgcGames.Application.Queries;

namespace FgcGames.Application.Validators;

public class GetAllGamesQueryValidator : AbstractValidator<GetAllGamesQuery>
{
    public GetAllGamesQueryValidator()
    {
        RuleFor(x => x.Pagina)
            .GreaterThanOrEqualTo(1)
                .WithMessage("A página deve ser maior ou igual a 1.");

        RuleFor(x => x.TamanhoPagina)
            .GreaterThan(0)
                .WithMessage("O tamanho da página deve ser maior que 0.")
            .LessThanOrEqualTo(50)
                .WithMessage("O tamanho máximo da página permitido é 50.");
    }
}
