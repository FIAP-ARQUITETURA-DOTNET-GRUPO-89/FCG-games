namespace FgcGames.Application.Responses;

public record GetUsersByNameResponse(
    Guid Id,
    string Nome,
    DateTime DataNascimento
);
