namespace FgcGames.Application.Queries;

public record GetAllUsersQuery(
    int Pagina = 1,
    int TamanhoPagina = 10,
    bool Ativos = true
);
