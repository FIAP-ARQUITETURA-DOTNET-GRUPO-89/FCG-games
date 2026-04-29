using FgcGames.Application.Commands;
using FgcGames.Application.Interfaces;
using FgcGames.Domain.Interfaces.Repositories;

namespace FgcGames.Application.Handlers;

public class UpdatePriceHandler(IGameRepository repository) : IUpdatePriceHandler
{
    public async Task Handle(UpdatePriceCommand command)
    {
        var jogo = await repository.ObterPorIdAsync(command.Id)
            ?? throw new KeyNotFoundException($"Jogo {command.Id} não encontrado.");

        jogo.AlterarPreco(command.NovoPreco);
        await repository.AtualizarAsync(jogo);
    }
}
