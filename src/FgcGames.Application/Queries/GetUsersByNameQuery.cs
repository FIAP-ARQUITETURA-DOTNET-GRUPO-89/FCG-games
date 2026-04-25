namespace FgcGames.Application.Queries;

public record GetUsersByNameQuery(
    string Nome,
    int Pagina = 1,
    int TamanhoPagina = 10
);
