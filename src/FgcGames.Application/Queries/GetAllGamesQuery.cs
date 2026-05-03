namespace FgcGames.Application.Queries;

public record GetAllGamesQuery(
    int Pagina = 1,
    int TamanhoPagina = 10
);
