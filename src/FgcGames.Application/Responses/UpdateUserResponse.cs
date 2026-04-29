namespace FgcGames.Application.Responses;

public record UpdateUserResponse(Guid Id, string Nome, DateOnly DataNascimento, string Mensagem);
