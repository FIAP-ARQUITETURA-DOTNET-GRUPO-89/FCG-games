using FgcGames.Domain.Entities;
using FgcGames.Domain.Interfaces.Repositories;
using FgcGames.Infra.Database;
using Microsoft.EntityFrameworkCore;

namespace FgcGames.Infra.Repositories;

public class UsuarioRepository(FgcGamesContext dbContext) : IUsuarioRepository
{
    private readonly FgcGamesContext _dbContext = dbContext;

    public async Task<Usuario?> ObterPorEmailAsync(string email)         
        => await _dbContext.Set<Usuario>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email.Endereco.ToLower() == email.ToLower());
}
