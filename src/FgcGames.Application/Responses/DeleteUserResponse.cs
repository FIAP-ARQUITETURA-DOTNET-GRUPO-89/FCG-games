namespace FgcGames.Application.Responses;

public record DeleteUserResponse(Guid Id, string Nome, bool Inativo, string Mensagem);
