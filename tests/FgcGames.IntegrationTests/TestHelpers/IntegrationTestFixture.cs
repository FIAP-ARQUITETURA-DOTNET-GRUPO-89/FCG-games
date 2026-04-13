using Aspire.Hosting;
using Aspire.Hosting.Testing;

namespace FgcGames.IntegrationTests.TestHelpers;

public class IntegrationTestFixture : IAsyncLifetime
{
    public DistributedApplication App { get; private set; } = default!;
    public HttpClient HttpClient { get; private set; } = default!;

    public async ValueTask InitializeAsync()
    {
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

    public async ValueTask DisposeAsync() => await App.DisposeAsync();
}
