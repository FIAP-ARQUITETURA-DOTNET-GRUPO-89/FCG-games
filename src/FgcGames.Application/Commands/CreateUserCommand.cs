namespace FgcGames.Application.Commands;

public record CreateUserCommand (string Nome, string Email, DateTime DataNascimento, string Senha)
{
}
