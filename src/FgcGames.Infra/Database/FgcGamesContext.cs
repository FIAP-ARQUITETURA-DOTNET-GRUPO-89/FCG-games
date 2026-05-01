using System.Reflection;
using FgcGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FgcGames.Infra.Database;

public class FgcGamesContext(DbContextOptions<FgcGamesContext> options) : DbContext(options)
{
    public DbSet<TaskItemExample> TaskItemExamples { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
