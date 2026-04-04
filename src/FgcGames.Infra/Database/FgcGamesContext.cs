using FgcGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class FgcGamesContext : DbContext
{
    public FgcGamesContext(DbContextOptions<FgcGamesContext> options)
        : base(options) { }

    public DbSet<Games> Games { get; set; }
}