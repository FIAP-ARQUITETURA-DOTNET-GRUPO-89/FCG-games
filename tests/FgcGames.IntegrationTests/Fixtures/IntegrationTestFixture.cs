using Aspire.Hosting;
using Aspire.Hosting.Testing;
using FgcGames.IntegrationTests.TestHelpers;
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

        var connectionString = await App.GetConnectionStringAsync("Default") ?? throw new InvalidOperationException("Connection string não encontrada");

        _dbManager = new TestDatabaseManager(connectionString);

        await _dbManager.InitializeAsync();
    }

    /// <summary>
    /// Finaliza a execução da aplicação após os testes.
    /// </summary>
    public async ValueTask DisposeAsync() => await App.DisposeAsync();

    /// <summary>
    /// Cria um HttpClient configurado para comunicação com a API.
    /// </summary>
    public HttpClient CreateClient()
        => App.CreateHttpClient("fgcgames-api", endpointName: "https");

    /// <summary>
    /// Reseta o banco de dados para um estado limpo.
    /// </summary>
    public Task ResetDatabaseAsync()
        => _dbManager.ResetAsync();
}
