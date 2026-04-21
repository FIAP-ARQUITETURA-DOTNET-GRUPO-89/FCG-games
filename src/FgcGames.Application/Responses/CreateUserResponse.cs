namespace FgcGames.Application.Responses;

public record CreateUserResponse(Guid Id, string Nome, DateTime DataNascimento, string Email);
