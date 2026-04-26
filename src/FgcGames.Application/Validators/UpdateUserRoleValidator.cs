using FgcGames.Application.Commands;
using FluentValidation;

namespace FgcGames.Application.Validators;

public class UpdateUserRoleValidator : AbstractValidator<UpdateUserRoleCommand>
{
    public UpdateUserRoleValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("O Id do usuário é obrigatório.");

        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("Role inválida.");
    }
}
