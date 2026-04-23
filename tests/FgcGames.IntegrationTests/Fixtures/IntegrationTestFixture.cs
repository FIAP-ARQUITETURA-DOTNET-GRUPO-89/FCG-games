using System.Net.Http.Headers;
using System.Net.Http.Json;
using Aspire.Hosting;
using Aspire.Hosting.Testing;
using FgcGames.Application.Responses;
using FgcGames.Infra.Database;
using FgcGames.Infra.Services;
using FgcGames.IntegrationTests.TestHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;

namespace FgcGames.IntegrationTests.Fixtures;

public class IntegrationTestFixture : IAsyncLifetime
{
    public DistributedApplication App { get; private set; } = default!;
    public HttpClient HttpClient { get; private set; } = default!;

    private string _connectionString = default!;
    private Respawner _respawner = default!;

    public async ValueTask InitializeAsync()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Testing");

        var builder = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.FgcGames_AppHost>();

        builder.Services.ConfigureHttpClientDefaults(client =>
        {
            client.ConfigurePrimaryHttpMessageHandler(() =>
                new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                });
        });

        App = await builder.BuildAsync();
        await App.StartAsync();

        HttpClient = App.CreateHttpClient("fgcgames-api", endpointName: "https");

        _connectionString = await App.GetConnectionStringAsync("Default")
            ?? throw new InvalidOperationException("Connection string não encontrada");

        await using (var context = CreateDbContext())
        {
            await context.Database.MigrateAsync();
        }

        await InitializeRespawner();
        await ResetDatabaseAsync();
    }

    public async ValueTask DisposeAsync() => await App.DisposeAsync();

    private async Task InitializeRespawner()
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        _respawner = await Respawner.CreateAsync(conn, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"],
            TablesToIgnore =
            [
                new("__EFMigrationsHistory")
            ]
        });
    }

    public async Task ResetDatabaseAsync()
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        await _respawner.ResetAsync(conn);

        await SeedAsync();
    }

    private async Task SeedAsync()
    {
        await using var context = CreateDbContext();

        var senhaHasher = new SenhaHasherService();

        await TestDataSeeder.SeedAsync(context, senhaHasher);
    }

    private FgcGamesContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<FgcGamesContext>()
            .UseNpgsql(_connectionString)
            .Options;

        return new FgcGamesContext(options);
    }

    public async Task AuthenticateAsAdminAsync()
    {
        ResetAuth();

        var token = await LoginAsync("admin@fgcgames.com", "Abc!1234");

        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task AuthenticateAsUserAsync()
    {
        ResetAuth();

        var token = await LoginAsync("user@fgcgames.com", "Abc!1234");

        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public void ResetAuth()
        => HttpClient.DefaultRequestHeaders.Authorization = null;

    public async Task<string> LoginAsync(string email, string senha)
    {
        var response = await HttpClient.PostAsJsonAsync("/auth", new
        {
            email,
            senha
        });

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

        return result!.Token;
    }
}
