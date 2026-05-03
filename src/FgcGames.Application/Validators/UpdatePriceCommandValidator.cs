using FluentValidation;
using FgcGames.Application.Commands;

namespace FgcGames.Application.Validators;

public class UpdatePriceCommandValidator : AbstractValidator<UpdatePriceCommand>
{
    public UpdatePriceCommandValidator()
    {
        RuleFor(x => x.NovoPreco)
            .GreaterThan(0)
                .WithMessage("O preço deve ser maior que zero.")
            .LessThanOrEqualTo(9999.99m)
                .WithMessage("O preço não pode ser superior a R$ 9.999,99.")
            .PrecisionScale(7, 2, false)
                .WithMessage("O preço deve ter no máximo 2 casas decimais.");
    }
}
