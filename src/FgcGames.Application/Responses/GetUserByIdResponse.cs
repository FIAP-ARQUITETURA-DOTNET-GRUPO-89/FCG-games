namespace FgcGames.Application.Responses;

public record GetUserByIdResponse(
    Guid Id,
    string Nome,
    string Email,
    DateOnly DataNascimento,
    bool Ativo
);
