using FgcGames.Domain.Entities;
using FgcGames.Domain.Interfaces.Repositories;
using FgcGames.Infra.Database;
using Microsoft.EntityFrameworkCore;

namespace FgcGames.Infra.Repositories;

public class JogoRepository(FgcGamesContext context) : IJogoRepository
{
    public async Task<Jogo?> ObterPorIdAsync(Guid id)
        => await context.Jogos.FirstOrDefaultAsync(j => j.Id == id);

    public async Task<IEnumerable<Jogo>> ObterTodosAsync()
        => await context.Jogos.Where(j => !j.Inativo).ToListAsync();

    public async Task AdicionarAsync(Jogo jogo)
    {
        await context.Jogos.AddAsync(jogo);
        await context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Jogo jogo)
    {
        context.Jogos.Update(jogo);
        await context.SaveChangesAsync();
    }

    public async Task RemoverAsync(Jogo jogo)
    {
        jogo.Inativar();
        context.Jogos.Update(jogo);
        await context.SaveChangesAsync();
    }
}
