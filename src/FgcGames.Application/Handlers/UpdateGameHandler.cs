using FgcGames.Application.Commands;
using FgcGames.Application.Interfaces;
using FgcGames.Domain.Interfaces.Repositories;
using FgcGames.Shared.Exceptions;

namespace FgcGames.Application.Handlers;

public class UpdateGameHandler(IGameRepository repository) : IUpdateGameHandler
{
    public async Task Handle(UpdateGameCommand command)
    {
        var game = await repository.ObterPorIdAsync(command.Id)
            ?? throw new NotFoundException($"Jogo {command.Id} não encontrado.");

        game.Atualizar(command.Nome, command.Descricao, command.DataLancamento, command.ClassificacaoEtaria);
        game.AlterarPreco(command.Preco);

        await repository.AtualizarAsync(game);
    }
}
