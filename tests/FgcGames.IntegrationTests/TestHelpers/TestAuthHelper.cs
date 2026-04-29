using System.Net.Http.Headers;
using System.Net.Http.Json;
using FgcGames.Application.Responses;
using FgcGames.IntegrationTests.Fixtures;

namespace FgcGames.IntegrationTests.TestHelpers;

/// <summary>
/// Responsável por facilitar a criação de HttpClients autenticados para testes de integração.
/// </summary>
public static class TestAuthHelper
{
    /// <summary>
    /// Cria um HttpClient autenticado como usuário Admin.
    /// Realiza o login na API e configura automaticamente o header Authorization com um token JWT válido com permissões de administrador.
    /// </summary>
    public static async Task<HttpClient> CreateAdminClientAsync(IntegrationTestFixture fixture)
    {
        var client = fixture.CreateClient();

        var token = await LoginAsync(client, "admin@fgcgames.com", "Abc!1234");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client;
    }

    /// <summary>
    /// Cria um HttpClient autenticado como usuário comum (User).
    /// Realiza o login na API e configura automaticamente o header Authorization com um token JWT válido com permissões de usuário padrão.
    /// </summary>
    public static async Task<HttpClient> CreateUserClientAsync(IntegrationTestFixture fixture)
    {
        var client = fixture.CreateClient();

        var token = await LoginAsync(client, "user@fgcgames.com", "Abc!1234");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client;
    }

    /// <summary>
    /// Cria um HttpClient sem autenticação.
    /// Útil para testar cenários onde o acesso deve ser negado (401 Unauthorized) ou endpoints públicos que não exigem autenticação.
    /// </summary>
    public static HttpClient CreateAnonymousClient(IntegrationTestFixture fixture)
        => fixture.CreateClient();

    /// <summary>
    /// Realiza o login na API e retorna um token JWT válido.
    /// </summary>
    private static async Task<string> LoginAsync(HttpClient client, string email, string senha)
    {
        var response = await client.PostAsJsonAsync("/auth", new
        {
            email,
            senha
        });

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

        return result!.Token;
    }
}
