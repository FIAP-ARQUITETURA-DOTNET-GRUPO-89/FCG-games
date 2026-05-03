using FgcGames.Domain.Enum;

namespace FgcGames.Application.Commands;

public record CreateUserCommand(string Nome, string Email, DateOnly DataNascimento, string Senha, UserRole Role = UserRole.User);
