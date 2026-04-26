using FgcGames.Domain.Enum;

namespace FgcGames.Application.Commands;

public record UpdateUserRoleCommand(Guid Id, UserRole Role);
