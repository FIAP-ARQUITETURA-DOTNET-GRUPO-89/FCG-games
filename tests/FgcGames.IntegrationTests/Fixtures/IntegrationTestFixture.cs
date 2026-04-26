using Aspire.Hosting;
using Aspire.Hosting.Testing;
using FgcGames.Infra.Database;
using Microsoft.Extensions.DependencyInjection;

namespace FgcGames.IntegrationTests.Fixtures;

public class IntegrationTestFixture : IAsyncLifetime
{
    public DistributedApplication App { get; private set; } = default!;
    public HttpClient HttpClient { get; private set; } = default!;

    public async ValueTask InitializeAsync()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Testing");

        var builder = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.FgcGames_AppHost>();

        App = await builder.BuildAsync();
        await App.StartAsync();

        var originalClient = App.CreateHttpClient("fgcgames-api");

        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        HttpClient = new HttpClient(handler)
        {
            BaseAddress = originalClient.BaseAddress
        };
    }

    public async Task ExecuteDbContextAsync(Func<FgcGamesContext, Task> action)
    {
        // Abre um escopo de Injeção de Dependência da aplicação orquestrada pelo Aspire
        using var scope = App.Services.CreateScope();

        // Recupera o Contexto do banco de dados real
        var context = scope.ServiceProvider.GetRequiredService<FgcGamesContext>();

        // Executa a tarefa (Ex: context.Usuarios.Add ou FirstOrDefault)
        await action(context);
    }

    public async ValueTask DisposeAsync() => await App.DisposeAsync();
}
