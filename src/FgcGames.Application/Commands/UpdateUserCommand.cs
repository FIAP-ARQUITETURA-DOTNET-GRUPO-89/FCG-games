namespace FgcGames.Application.Commands;

public record UpdateUserCommand(Guid Id, string Nome, DateTime DataNascimento)
{
}
