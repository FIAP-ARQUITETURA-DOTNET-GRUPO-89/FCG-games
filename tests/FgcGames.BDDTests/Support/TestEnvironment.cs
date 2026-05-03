using Aspire.Hosting;
using Aspire.Hosting.Testing;
using FgcGames.Infra.Database;
using FgcGames.Infra.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Respawn;

namespace FgcGames.BDDTests.Support;

public static class TestEnvironment
{
    public static DistributedApplication App { get; private set; } = default!;
    public static string ConnectionString { get; private set; } = string.Empty;
    private static Respawner _respawner = default!;

    public static async Task InitializeAsync()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Testing");

        var builder = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.FgcGames_AppHost>();

        App = await builder.BuildAsync();
        await App.StartAsync();

        ConnectionString = await App.GetConnectionStringAsync("Default") ?? string.Empty;

        await using var context = CreateDbContext();
        await context.Database.MigrateAsync();

        await InitializeRespawnerAsync();
        await SeedAsync();
    }

    public static async Task ResetAsync()
    {
        await using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();
        await _respawner.ResetAsync(conn);
        await SeedAsync();
    }

    public static HttpClient CreateClient()
        => App.CreateHttpClient("fgcgames-api");

    public static async Task DisposeAsync()
    {
        await App.StopAsync();
        await App.DisposeAsync();
    }

    private static async Task InitializeRespawnerAsync()
    {
        await using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();

        _respawner = await Respawner.CreateAsync(conn, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"],
            TablesToIgnore = [new("__EFMigrationsHistory")]
        });
    }

    private static async Task SeedAsync()
    {
        await using var context = CreateDbContext();
        var senhaHasher = new SenhaHasherService();

        if (!await context.Usuarios.AnyAsync())
        {
            var admin = new FgcGames.Domain.Entities.Usuario(
                nome: "Admin",
                dataNascimento: new DateOnly(1990, 1, 1),
                email: new FgcGames.Domain.ValueObjects.Email("admin@fgcgames.com"),
                senha: FgcGames.Domain.ValueObjects.Senha.FromHash(senhaHasher.Hash("Abc!1234")),
                userRole: FgcGames.Domain.Enum.UserRole.Admin
            );

            var user = new FgcGames.Domain.Entities.Usuario(
                nome: "User",
                dataNascimento: new DateOnly(1995, 1, 1),
                email: new FgcGames.Domain.ValueObjects.Email("user@fgcgames.com"),
                senha: FgcGames.Domain.ValueObjects.Senha.FromHash(senhaHasher.Hash("Abc!1234")),
                userRole: FgcGames.Domain.Enum.UserRole.User
            );

            context.Usuarios.AddRange(admin, user);
            await context.SaveChangesAsync();
        }
    }

    private static FgcGamesContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<FgcGamesContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        return new FgcGamesContext(options);
    }
}
