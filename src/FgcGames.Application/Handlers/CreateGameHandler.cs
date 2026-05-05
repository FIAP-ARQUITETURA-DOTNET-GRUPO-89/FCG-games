using FgcGames.Application.Commands;
using FgcGames.Application.Interfaces;
using FgcGames.Application.Responses;
using FgcGames.Domain.Entities;
using FgcGames.Domain.Interfaces.Repositories;

namespace FgcGames.Application.Handlers;

public class CreateGameHandler(IGameRepository repository) : ICreateGameHandler
{
    public async Task<GameResponse> Handle(CreateGameCommand command)
    {
        var jogo = new Jogo(
            command.DataLancamento,
            command.Nome,
            command.Descricao,
            command.Preco,
            command.ClassificacaoEtaria
        );

        await repository.AdicionarAsync(jogo);

        return new GameResponse(
            jogo.Id,
            jogo.Nome,
            jogo.Descricao,
            jogo.Preco,
            jogo.DataLancamento,
            jogo.ClassificacaoEtaria
        );
    }
}
