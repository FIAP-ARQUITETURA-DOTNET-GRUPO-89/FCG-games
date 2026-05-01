using Aspire.Hosting;
using Aspire.Hosting.Testing;
using FgcGames.Infra.Database;
using FgcGames.IntegrationTests.TestHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FgcGames.IntegrationTests.Fixtures;

/// <summary>
/// Fixture base para testes de integração da aplicação.
/// Essa classe atua como ponto central de orquestração da infraestrutura de testes, permitindo que os testes foquem apenas no comportamento da aplicação.
/// </summary>
public class IntegrationTestFixture : IAsyncLifetime
{
    public DistributedApplication App { get; private set; } = default!;

    private TestDatabaseManager _dbManager = default!;
    private string _connectionString = string.Empty;
    private HttpClient? _httpClient;

    public HttpClient HttpClient => _httpClient ??= CreateClient();

    /// <summary>
    /// Inicializa o ambiente de testes.
    /// </summary>
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

        _connectionString = await App.GetConnectionStringAsync("Default") ?? string.Empty;
        _dbManager = new TestDatabaseManager(_connectionString);
        await _dbManager.InitializeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        _httpClient?.Dispose();
        if (App is not null)
        {
            await App.StopAsync();
            await App.DisposeAsync();
        }
    }

    public async Task ResetDatabaseAsync()
        => await _dbManager.ResetAsync();

    public HttpClient CreateClient()
        => App.CreateHttpClient("fgcgames-api");

    public async Task ExecuteDbContextAsync(Func<FgcGamesContext, Task> action)
    {
        var options = new DbContextOptionsBuilder<FgcGamesContext>()
            .UseNpgsql(_connectionString)
            .Options;

        await using var context = new FgcGamesContext(options);
        await action(context);
        await context.SaveChangesAsync();
    }
}
