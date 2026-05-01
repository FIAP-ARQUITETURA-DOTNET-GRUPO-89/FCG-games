using FgcGames.Application.Commands;
using FluentValidation;

namespace FgcGames.Application.Validators;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(a => a.Email)
            .NotEmpty()
            .Length(1, 254)
            .EmailAddress().WithMessage("O formato do email é inválido.");

        RuleFor(a => a.Senha)
            .NotEmpty().WithMessage("'Senha' deve ser informada.")
            .Length(8, 12).WithMessage("'Senha' deve ter entre 8 e 12 caracteres. Você digitou {TotalLength} caracteres.")
            .Matches(@"[A-Z]").WithMessage("A senha deve conter pelo menos uma letra maiúscula.")
            .Matches(@"[a-z]").WithMessage("A senha deve conter pelo menos uma letra minúscula.")
            .Matches(@"[0-9]").WithMessage("A senha deve conter pelo menos um número.")
            .Matches(@"[!?*.@#$%&]").WithMessage("A senha deve conter pelo menos um caractere especial. Exemplos permitidos: ! ? * . @ # $ % &");
    }
}
