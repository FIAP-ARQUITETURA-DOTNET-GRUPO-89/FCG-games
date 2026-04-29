using Aspire.Hosting;
using Aspire.Hosting.Testing;
using FgcGames.Infra.Database;
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

        try
        {
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
        }
        catch (Exception ex) when (IsContainerRuntimeUnavailable(ex))
        {
            Assert.Skip("Docker/Container runtime não disponível. Testes de integração foram ignorados.");
            return;
        }

        var connectionString = await App.GetConnectionStringAsync("Default") ?? throw new InvalidOperationException("Connection string não encontrada");

        _dbManager = new TestDatabaseManager(connectionString);

        await _dbManager.InitializeAsync();
    }

    public async Task ExecuteDbContextAsync(Func<FgcGamesContext, Task> action)
    {
        using var scope = App.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FgcGamesContext>();
        await action(context);
    }

    /// <summary>
    /// Finaliza a execução da aplicação após os testes.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (App is not null)
            await App.DisposeAsync();
    }

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

    private static bool IsContainerRuntimeUnavailable(Exception exception)
        => exception.ToString().Contains(
            "Container runtime 'docker' could not be found",
            StringComparison.OrdinalIgnoreCase);
}
