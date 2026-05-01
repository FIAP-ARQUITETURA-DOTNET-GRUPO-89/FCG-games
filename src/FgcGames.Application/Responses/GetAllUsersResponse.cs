namespace FgcGames.Application.Responses;

public record GetAllUsersResponse(
    Guid Id,
    string Nome,
    string Email,
    DateOnly DataNascimento,
    bool Ativo
);
