using FgcGames.Application.Commands;
using FgcGames.Application.Interfaces;
using FgcGames.Domain.Interfaces.Repositories;

namespace FgcGames.Application.Handlers;

public class DeleteGameHandler(IGameRepository repository) : IDeleteGameHandler
{
    public async Task Handle(DeleteGameCommand command)
    {
        var jogo = await repository.ObterPorIdAsync(command.Id)
            ?? throw new KeyNotFoundException($"Jogo {command.Id} não encontrado.");

        await repository.RemoverAsync(jogo);
    }
}
