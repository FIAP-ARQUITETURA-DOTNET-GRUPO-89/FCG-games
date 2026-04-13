using FgcGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FgcGames.Infra.Database;

public class FgcGamesContext(DbContextOptions<FgcGamesContext> options) : DbContext(options)
{
    public DbSet<Games> Games { get; set; }

    public DbSet<TaskItemExample> TaskItemExamples { get; set; }
}
