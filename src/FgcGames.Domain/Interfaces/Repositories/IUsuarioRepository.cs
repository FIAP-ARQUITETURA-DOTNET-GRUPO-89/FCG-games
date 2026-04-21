using FgcGames.Domain.Entities;

namespace FgcGames.Domain.Interfaces.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorEmailAsync(string email);
}
