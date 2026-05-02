namespace FgcGames.Application.Responses;

public record CreateUserResponse(Guid Id, string Nome, DateOnly DataNascimento, string Email);
