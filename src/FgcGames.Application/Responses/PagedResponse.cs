namespace FgcGames.Application.Responses;

public record PagedResponse<T>(IEnumerable<T> Itens, int PaginaAtual, int TotalPaginas, int TotalItens);
