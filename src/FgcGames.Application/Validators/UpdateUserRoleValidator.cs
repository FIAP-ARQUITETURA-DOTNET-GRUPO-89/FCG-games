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

        RuleFor(x => x.RoleName)
            .NotEmpty()
            .Must(role => role.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                         role.Equals("User", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Role inválida. Use 'Admin' ou 'User'.");
    }
}
