using FgcGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FgcGames.Infra.Database;

public class FgcGamesContext(DbContextOptions<FgcGamesContext> options) : DbContext(options)
{
    public DbSet<TaskItemExample> TaskItemExamples { get; set; }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Jogo> Jogos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FgcGamesContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
