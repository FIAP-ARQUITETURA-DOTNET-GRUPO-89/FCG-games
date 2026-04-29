using FgcGames.Application.Commands;
using FluentValidation;

namespace FgcGames.Application.Validators;

public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator()
    {
        RuleFor(a => a.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MinimumLength(2).WithMessage("O nome deve ter pelo menos 2 caracteres.")
            .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres.")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$").WithMessage("O nome deve conter apenas letras.");

        RuleFor(a => a.DataNascimento)
            .NotEmpty().WithMessage("A data de nascimento é obrigatória.")
            .LessThan(DateTime.Today).WithMessage("A data de nascimento deve ser no passado.");

        //RuleFor(a => a.Email)
        //    .NotEmpty()
        //    .Length(1, 254)
        //    .EmailAddress().WithMessage("O formato do email é inválido.");

        //RuleFor(a => a.Senha)
        //    .NotEmpty()
        //    .Length(8, 12)
        //    .Matches(@"[A-Z]").WithMessage("A senha deve conter pelo menos uma letra maiúscula.")
        //    .Matches(@"[a-z]").WithMessage("A senha deve conter pelo menos uma letra minúscula.")
        //    .Matches(@"[0-9]").WithMessage("A senha deve conter pelo menos um número.")
        //    .Matches(@"[!?*.]").WithMessage("A senha deve conter pelo menos um caractere especial (!? *.).");
    }
}
