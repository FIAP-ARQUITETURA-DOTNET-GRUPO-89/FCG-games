using FgcGames.Application.Interfaces;
using FgcGames.Application.Queries;
using FgcGames.Application.Responses;
using FgcGames.Domain.Interfaces.Repositories;

namespace FgcGames.Application.Handlers;

public class GetGameByIdHandler(IGameRepository repository) : IGetGameByIdHandler
{
    public async Task<GameResponse> Handle(GetGameByIdQuery query)
    {
        var jogo = await repository.ObterPorIdAsync(query.Id)
            ?? throw new KeyNotFoundException($"Jogo {query.Id} não encontrado.");

        return new GameResponse(
            jogo.Id, jogo.Nome, jogo.Descricao,
            jogo.Preco, jogo.DataLancamento, jogo.ClassificacaoEtaria
        );
    }
}
