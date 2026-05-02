using FluentValidation;
using FgcGames.Application.Commands;

namespace FgcGames.Application.Validators;

public class UpdatePasswordValidator : AbstractValidator<UpdatePasswordCommand>
{
    public UpdatePasswordValidator()
    {
        RuleFor(a => a.Password)
            .NotEmpty()
            .Length(8, 12)
            .Matches(@"[A-Z]").WithMessage("A senha deve conter pelo menos uma letra maiúscula.")
            .Matches(@"[a-z]").WithMessage("A senha deve conter pelo menos uma letra minúscula.")
            .Matches(@"[0-9]").WithMessage("A senha deve conter pelo menos um número.")
            .Matches(@"[!?\*\.@#$%&]").WithMessage("A senha deve conter pelo menos um caractere especial. Exemplos permitidos: ! ? * . @ # $ % &");
    }
}
