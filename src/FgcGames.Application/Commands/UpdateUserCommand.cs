namespace FgcGames.Application.Commands;

public record UpdateUserCommand(string Nome, string Email, DateTime DataNascimento, string Senha)
{
}
