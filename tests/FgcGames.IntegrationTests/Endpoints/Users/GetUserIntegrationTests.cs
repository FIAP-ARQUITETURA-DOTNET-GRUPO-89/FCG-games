using System.Net;
using System.Net.Http.Json;
using FgcGames.Application.Responses;
using FgcGames.Domain.Entities;
using FgcGames.Domain.ValueObjects;
using FgcGames.Domain.Enum;
using FgcGames.IntegrationTests.Fixtures;
using Shouldly;

namespace FgcGames.IntegrationTests.Endpoints.Users;

public class GetUserIntegrationTests(IntegrationTestFixture fixture) : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client = fixture.HttpClient;

    [Fact]
    public async Task Dado_NomeExistenteNoBanco_Quando_BuscarUsuariosPorNome_Entao_RetornaListaPaginadaCorreta()
    {
        // ARRANGE
        var termoBusca = "Silva";

        await fixture.ExecuteDbContextAsync(async (context) =>
        {
            context.Usuarios.RemoveRange(context.Usuarios);

            var usuarios = new List<Usuario>
            {
                new Usuario("Alice Silva", new DateOnly(1990, 5, 10), Email.Create("alice@email.com"), Senha.FromHash("Senha@123"), UserRole.User),
                new Usuario("Bruno Silva", new DateOnly(1985, 3, 20), Email.Create("bruno@email.com"), Senha.FromHash("Senha@123"), UserRole.User),
                new Usuario("Carlos Oliveira", new DateOnly(2000, 1, 1), Email.Create("carlos@email.com"), Senha.FromHash("Senha@123"), UserRole.User)
            };

            await context.Usuarios.AddRangeAsync(usuarios);
            await context.SaveChangesAsync();
        });

        var url = $"/usuarios?nome={termoBusca}&pagina=1&tamanhoPagina=10";

        // ACT
        var response = await _client.GetAsync(url, cancellationToken: TestContext.Current.CancellationToken);

        // ASSERT
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResponse<GetUsersByNameResponse>>(cancellationToken: TestContext.Current.CancellationToken);

        result.ShouldNotBeNull();
        result.Itens.Count().ShouldBe(2);
        result.TotalItens.ShouldBe(2);
        result.PaginaAtual.ShouldBe(1);
        result.Itens.ShouldAllBe(u => u.Nome.Contains(termoBusca));
    }

    [Fact]
    public async Task Dado_UsuarioInativoNoBanco_Quando_BuscarPorNome_Entao_NaoDeveExibirNaLista()
    {
        // ARRANGE
        var nomeInativo = "Daniel Souza";

        await fixture.ExecuteDbContextAsync(async (context) =>
        {
            context.Usuarios.RemoveRange(context.Usuarios);

            var usuarioInativo = new Usuario(
                nomeInativo,
                new DateOnly(1992, 8, 15),
                Email.Create("daniel@email.com"),
                Senha.FromHash("Senha@123"),
                UserRole.User
            );

            var property = typeof(Usuario).GetProperty("Inativo");
            property?.SetValue(usuarioInativo, true);

            context.Usuarios.Add(usuarioInativo);
            await context.SaveChangesAsync();
        });

        // ACT
        var response = await _client.GetAsync($"/usuarios?nome={nomeInativo}&pagina=1&tamanhoPagina=10", TestContext.Current.CancellationToken);

        // ASSERT
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResponse<GetUsersByNameResponse>>(cancellationToken: TestContext.Current.CancellationToken);
        
        result.ShouldNotBeNull();
        result.Itens.ShouldBeEmpty();
        result.TotalItens.ShouldBe(0);
    }

    [Fact]
    public async Task Dado_NomeVazio_Quando_BuscarUsuarios_Entao_RetornaResultadoPaginadoVazio()
    {
        // ARRANGE
        await fixture.ExecuteDbContextAsync(async (context) =>
        {
            context.Usuarios.RemoveRange(context.Usuarios);

            var usuarioQualquer = new Usuario(
                "João Silva",
                new DateOnly(1990, 1, 1),
                Email.Create("joao@teste.com"),
                Senha.FromHash("Senha@123"),
                UserRole.User
            );

            context.Usuarios.Add(usuarioQualquer);
            await context.SaveChangesAsync();
        });

        // ACT
        var response = await _client.GetAsync("/usuarios?nome=&pagina=1&tamanhoPagina=10", TestContext.Current.CancellationToken);

        // ASSERT
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResponse<GetUsersByNameResponse>>(cancellationToken: TestContext.Current.CancellationToken);

        result.ShouldNotBeNull();
        result.Itens.ShouldBeEmpty();
        result.TotalItens.ShouldBe(0);
    }
}
