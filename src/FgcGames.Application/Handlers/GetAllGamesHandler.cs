using FgcGames.Application.Interfaces;
using FgcGames.Application.Queries;
using FgcGames.Application.Responses;
using FgcGames.Domain.Interfaces.Repositories;

namespace FgcGames.Application.Handlers;

public class GetAllGamesHandler(IGameRepository repository) : IGetAllGamesHandler
{
    public async Task<IEnumerable<GameResponse>> Handle(GetAllGamesQuery query)
    {
        var jogos = await repository.ObterTodosAsync();

        return jogos.Select(j => new GameResponse(
            j.Id, j.Nome, j.Descricao,
            j.Preco, j.DataLancamento, j.ClassificacaoEtaria
        ));
    }
}
