using FgcGames.Domain.Enum;

namespace FgcGames.Application.Responses;

public record UpdateUserRoleResponse(Guid Id, UserRole Role, string Mensagem);
