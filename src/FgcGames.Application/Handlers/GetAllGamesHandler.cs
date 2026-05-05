using FgcGames.Application.Interfaces;
using FgcGames.Application.Queries;
using FgcGames.Application.Responses;
using FgcGames.Domain.Interfaces.Repositories;

namespace FgcGames.Application.Handlers;

public class GetAllGamesHandler(IGameRepository repository) : IGetAllGamesHandler
{
    public async Task<PagedResponse<GameResponse>> Handle(GetAllGamesQuery query)
    {
        var total = await repository.ContarAtivosAsync();
        var jogos = await repository.ObterPaginadoAsync(query.Pagina, query.TamanhoPagina);

        var itens = jogos.Select(j => new GameResponse(
            j.Id, j.Nome, j.Descricao,
            j.Preco, j.DataLancamento, j.ClassificacaoEtaria
        ));

        var totalPaginas = (int)Math.Ceiling(total / (double)query.TamanhoPagina);

        return new PagedResponse<GameResponse>(itens, query.Pagina, totalPaginas, total);
    }
}
