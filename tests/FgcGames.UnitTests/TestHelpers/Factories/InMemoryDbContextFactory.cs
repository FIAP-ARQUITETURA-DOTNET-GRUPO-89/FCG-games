using FgcGames.Infra.Database;
using Microsoft.EntityFrameworkCore;

namespace FgcGames.UnitTests.TestHelpers.Factories;

public static class InMemoryDbContextFactory
{
    public static FgcGamesContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<FgcGamesContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new FgcGamesContext(options);
    }
}
