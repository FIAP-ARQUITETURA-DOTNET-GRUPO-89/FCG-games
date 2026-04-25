using FgcGames.Domain.Entities;
using FgcGames.Domain.Interfaces.Repositories;
using FgcGames.Infra.Database;
using Microsoft.EntityFrameworkCore;

namespace FgcGames.Infra.Repositories;

public class UsuarioRepository(FgcGamesContext context) : BaseRepository<Usuario>(context), IUsuarioRepository
{
    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Set<Usuario>()
            .AnyAsync(u => u.Email.Endereco == email);
    }
}
