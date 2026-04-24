namespace FgcGames.Application.Responses;

public record UpdateUserResponse(Guid Id, string Nome, DateTime DataNascimento, string Mensagem);
