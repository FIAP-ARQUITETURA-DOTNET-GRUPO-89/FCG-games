using System.Net;
using System.Net.Http.Json;
using FgcGames.Application.Responses;
using FgcGames.Domain.Entities;
using FgcGames.Domain.ValueObjects;
using FgcGames.Domain.Enum;
using FgcGames.IntegrationTests.Fixtures;
using FgcGames.IntegrationTests.TestHelpers;
using Shouldly;

namespace FgcGames.IntegrationTests.Endpoints.Users;

[Collection("IntegrationTests")]
public class GetUserIntegrationTests(IntegrationTestFixture fixture) : IAsyncLifetime
{
    private readonly IntegrationTestFixture _fixture = fixture;
    private HttpClient _client = default!;

    public async ValueTask InitializeAsync()
    {
        await _fixture.ResetDatabaseAsync();
        _client = await TestAuthHelper.CreateAdminClientAsync(_fixture);
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task Dado_NomeExistenteNoBanco_Quando_BuscarUsuariosPorNome_Entao_RetornaListaPaginadaCorreta()
    {
        // ARRANGE
        var termoBusca = "Oliveira";

        await _fixture.ExecuteDbContextAsync(async (context) =>
        {
            context.Usuarios.RemoveRange(context.Usuarios);

            var usuarios = new List<Usuario>
            {
                new Usuario("Marcos Oliveira", new DateOnly(1990, 5, 10), Email.Create("marcos@email.com"), Senha.FromHash("Senha@123"), UserRole.User),
                new Usuario("Fernanda Oliveira", new DateOnly(1985, 3, 20), Email.Create("fernanda@email.com"), Senha.FromHash("Senha@123"), UserRole.User),
                new Usuario("Lucas Santos", new DateOnly(2000, 1, 1), Email.Create("lucas@email.com"), Senha.FromHash("Senha@123"), UserRole.User)
            };

            await context.Usuarios.AddRangeAsync(usuarios);
            await context.SaveChangesAsync();
        });

        var url = $"/usuarios/busca?nome={termoBusca}&pagina=1&tamanhoPagina=10";

        // ACT
        var response = await _client.GetAsync(url, TestContext.Current.CancellationToken);

        // ASSERT
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResponse<GetUsersByNameResponse>>(TestContext.Current.CancellationToken);

        result.ShouldNotBeNull();
        result.Itens.Count().ShouldBe(2);
        result.TotalItens.ShouldBe(2);
        result.Itens.ShouldAllBe(u => u.Nome.Contains(termoBusca));
    }

    [Fact]
    public async Task Dado_UsuarioInativoNoBanco_Quando_BuscarPorNome_Entao_NaoDeveExibirNaLista()
    {
        // ARRANGE
        var nomeInativo = "Roberto Almeida";

        await _fixture.ExecuteDbContextAsync(async (context) =>
        {
            context.Usuarios.RemoveRange(context.Usuarios);

            var usuarioInativo = new Usuario(
                nomeInativo,
                new DateOnly(1992, 8, 15),
                Email.Create("roberto@email.com"),
                Senha.FromHash("Senha@123"),
                UserRole.User
            );

            usuarioInativo.Inativar();

            context.Usuarios.Add(usuarioInativo);
            await context.SaveChangesAsync();
        });

        // ACT
        var response = await _client.GetAsync($"/usuarios/busca?nome={nomeInativo}&pagina=1&tamanhoPagina=10", TestContext.Current.CancellationToken);

        // ASSERT
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResponse<GetUsersByNameResponse>>(TestContext.Current.CancellationToken);

        result.ShouldNotBeNull();
        result.Itens.ShouldBeEmpty();
        result.TotalItens.ShouldBe(0);
    }

    [Fact]
    public async Task Dado_NomeVazio_Quando_BuscarUsuarios_Entao_RetornaResultadoPaginadoVazio()
    {
        // ARRANGE
        await _fixture.ExecuteDbContextAsync(async (context) =>
        {
            context.Usuarios.RemoveRange(context.Usuarios);

            var usuarioQualquer = new Usuario(
                "Mariana Costa",
                new DateOnly(1990, 1, 1),
                Email.Create("mariana@teste.com"),
                Senha.FromHash("Senha@123"),
                UserRole.User
            );

            context.Usuarios.Add(usuarioQualquer);
            await context.SaveChangesAsync();
        });

        // ACT
        var response = await _client.GetAsync("/usuarios/busca?nome=&pagina=1&tamanhoPagina=10", TestContext.Current.CancellationToken);

        // ASSERT
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResponse<GetUsersByNameResponse>>(TestContext.Current.CancellationToken);

        result.ShouldNotBeNull();
        result.Itens.ShouldBeEmpty();
        result.TotalItens.ShouldBe(0);
    }

    [Fact]
    public async Task Dado_ExistemUsuariosNoBanco_Quando_ListarTodos_Entao_RetornaListaPaginadaComTodosOsAtivos()
    {
        // ARRANGE
        await _fixture.ExecuteDbContextAsync(async (context) =>
        {
            context.Usuarios.RemoveRange(context.Usuarios);
            await context.Usuarios.AddRangeAsync(
                new Usuario("Teste Um", new DateOnly(1990, 1, 1), Email.Create("teste1@email.com"), Senha.FromHash("Senha@123"), UserRole.User),
                new Usuario("Teste Dois", new DateOnly(1990, 1, 1), Email.Create("teste2@email.com"), Senha.FromHash("Senha@123"), UserRole.User)
            );
            await context.SaveChangesAsync();
        });

        // ACT
        var response = await _client.GetAsync("/usuarios?pagina=1&tamanhoPagina=10", TestContext.Current.CancellationToken);

        // ASSERT
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<GetAllUsersResponse>>(TestContext.Current.CancellationToken);

        result.ShouldNotBeNull();
        result.TotalItens.ShouldBeGreaterThanOrEqualTo(2);
        result.Itens.Count().ShouldBe(2);
    }

    [Fact]
    public async Task Dado_IdValidoExistente_Quando_BuscarPorId_Entao_RetornaUsuarioCorreto()
    {
        // ARRANGE
        var usuarioOriginal = new Usuario("Usuario Exemplo", new DateOnly(1995, 1, 1), Email.Create("exemplo@teste.com"), Senha.FromHash("Senha@123"), UserRole.Admin);

        await _fixture.ExecuteDbContextAsync(async (context) =>
        {
            context.Usuarios.Add(usuarioOriginal);
            await context.SaveChangesAsync();
        });

        // ACT
        var response = await _client.GetAsync($"/usuarios/{usuarioOriginal.Id}", TestContext.Current.CancellationToken);

        // ASSERT
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<GetUserByIdResponse>(TestContext.Current.CancellationToken);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(usuarioOriginal.Id);
        result.Nome.ShouldBe(usuarioOriginal.Nome);
        result.Email.ShouldBe(usuarioOriginal.Email.Endereco);
    }

    [Fact]
    public async Task Dado_IdInexistente_Quando_BuscarPorId_Entao_RetornaNotFound()
    {
        // ARRANGE
        var idInexistente = Guid.NewGuid();

        // ACT
        var response = await _client.GetAsync($"/usuarios/{idInexistente}", TestContext.Current.CancellationToken);

        // ASSERT
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
