using FgcGames.Domain.Entities;

namespace FgcGames.Domain.Interfaces.Repositories;

public interface IGameRepository
{
    Task<Jogo?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<Jogo>> ObterTodosAsync();
    Task<bool> ExisteComNomeAsync(string nome, Guid? ignorarId = null);
    Task<int> ContarAtivosAsync();
    Task<IReadOnlyList<Jogo>> ObterPaginadoAsync(int pagina, int tamanhoPagina);
    Task AdicionarAsync(Jogo jogo);
    Task AtualizarAsync(Jogo jogo);
    Task RemoverAsync(Jogo jogo);
}
