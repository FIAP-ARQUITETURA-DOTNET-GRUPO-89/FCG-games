using FgcGames.Domain.Entities;

namespace FgcGames.Domain.Interfaces.Repositories;

public interface IUsuarioRepository : IBaseRepository<Usuario>
{
    Task<bool> ExistsByEmailAsync(string email);
}
