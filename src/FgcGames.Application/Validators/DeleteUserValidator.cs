using FgcGames.Application.Commands;
using FluentValidation;

namespace FgcGames.Application.Validators;

public class DeleteUserValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("O Id do usuário é obrigatório e deve ser um identificador válido.");
    }
}
