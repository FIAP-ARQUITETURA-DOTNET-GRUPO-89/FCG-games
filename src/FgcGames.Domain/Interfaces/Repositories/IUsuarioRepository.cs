using FgcGames.Domain.Entities;

namespace FgcGames.Domain.Interfaces.Repositories;

public interface IUsuarioRepository : IBaseRepository<Usuario>
{
    /// <summary>
    /// Obtém um usuário a partir do e-mail informado.
    /// </summary>
    /// <param name="email">E-mail do usuário.</param>
    /// <returns>Usuário encontrado ou null caso não exista.</returns>
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task<bool> ExistsByEmailAsync(string email);
}
