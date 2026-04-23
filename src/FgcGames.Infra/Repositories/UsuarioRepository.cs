using FgcGames.Domain.Entities;
using FgcGames.Domain.Interfaces.Repositories;
using FgcGames.Infra.Database;
using Microsoft.EntityFrameworkCore;

namespace FgcGames.Infra.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly FgcGamesContext _context;

    public UsuarioRepository(FgcGamesContext context)
    {
        _context = context;
    }

    public void Add(Usuario entity) => throw new NotImplementedException();

    public void Delete(Usuario entity) => throw new NotImplementedException();

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Set<Usuario>()
            .AnyAsync(u => u.Email.Endereco == email);
    }

    public Task<IReadOnlyList<Usuario>> GetAllAsync() => throw new NotImplementedException();

    //public Task<Usuario?> GetByIdAsync(int id) => throw new NotImplementedException();
    public Task<Usuario?> GetByIdAsync(Guid id) => throw new NotImplementedException();

    public Task<int> SaveChangesAsync() => throw new NotImplementedException();

    public void Update(Usuario entity) => throw new NotImplementedException();
}
