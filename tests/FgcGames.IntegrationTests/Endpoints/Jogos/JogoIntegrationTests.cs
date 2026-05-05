using System.Net;
using System.Net.Http.Json;
using FgcGames.Application.Commands;
using FgcGames.Application.Responses;
using FgcGames.Domain.Enum;
using FgcGames.IntegrationTests.Fixtures;
using FgcGames.IntegrationTests.TestHelpers;
using Shouldly;

namespace FgcGames.IntegrationTests.Endpoints.Jogos;

[Collection("IntegrationTests")]
public class JogoIntegrationTests(IntegrationTestFixture fixture) : IAsyncLifetime
{
    private readonly IntegrationTestFixture _fixture = fixture;

    public async ValueTask InitializeAsync() => await _fixture.ResetDatabaseAsync();
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    private static CreateGameCommand NovoJogo(string nome = "The Witcher 3") => new(
        nome,
        "Um excelente RPG de mundo aberto.",
        199.90m,
        new DateTime(2015, 5, 19, 0, 0, 0, DateTimeKind.Utc),
        ClassificacaoEtaria.Dezoito
    );

    private static async Task<GameResponse> CriarJogoAsync(HttpClient client, string nome = "The Witcher 3")
    {
        var response = await client.PostAsJsonAsync("/jogos", NovoJogo(nome), TestContext.Current.CancellationToken);
        return (await response.ReadContentAsync<GameResponse>(TestContext.Current.CancellationToken))!;
    }

    // ── POST /jogos ──────────────────────────────────────────────────────────

    [Fact]
    public async Task Dado_DadosValidos_Quando_AdminCriarJogo_Entao_Retorna201()
    {
        var client = await TestAuthHelper.CreateAdminClientAsync(_fixture);

        var response = await client.PostAsJsonAsync("/jogos", NovoJogo(), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var result = await response.ReadContentAsync<GameResponse>(TestContext.Current.CancellationToken);
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
        result.Nome.ShouldBe("The Witcher 3");
        result.Preco.ShouldBe(199.90m);
    }

    [Fact]
    public async Task Dado_UserSemPermissao_Quando_CriarJogo_Entao_Retorna403()
    {
        var client = await TestAuthHelper.CreateUserClientAsync(_fixture);

        var response = await client.PostAsJsonAsync("/jogos", NovoJogo(), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Dado_SemAutenticacao_Quando_CriarJogo_Entao_Retorna401()
    {
        var client = TestAuthHelper.CreateAnonymousClient(_fixture);

        var response = await client.PostAsJsonAsync("/jogos", NovoJogo(), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    // ── GET /jogos ───────────────────────────────────────────────────────────

    [Fact]
    public async Task Dado_JogosExistentes_Quando_AdminListar_Entao_Retorna200ComLista()
    {
        var client = await TestAuthHelper.CreateAdminClientAsync(_fixture);
        await CriarJogoAsync(client, "Jogo A");
        await CriarJogoAsync(client, "Jogo B");

        var response = await client.GetAsync("/jogos", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.ReadContentAsync<PagedResponse<GameResponse>>(TestContext.Current.CancellationToken);
        result.ShouldNotBeNull();
        result.Itens.Count().ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task Dado_Autenticado_Quando_UserListarJogos_Entao_Retorna200()
    {
        var adminClient = await TestAuthHelper.CreateAdminClientAsync(_fixture);
        await CriarJogoAsync(adminClient);

        var userClient = await TestAuthHelper.CreateUserClientAsync(_fixture);
        var response = await userClient.GetAsync("/jogos", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Dado_SemAutenticacao_Quando_ListarJogos_Entao_Retorna401()
    {
        var client = TestAuthHelper.CreateAnonymousClient(_fixture);

        var response = await client.GetAsync("/jogos", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    // ── GET /jogos/{id} ──────────────────────────────────────────────────────

    [Fact]
    public async Task Dado_JogoExistente_Quando_BuscarPorId_Entao_Retorna200()
    {
        var client = await TestAuthHelper.CreateAdminClientAsync(_fixture);
        var criado = await CriarJogoAsync(client);

        var response = await client.GetAsync($"/jogos/{criado.Id}", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.ReadContentAsync<GameResponse>(TestContext.Current.CancellationToken);
        result.ShouldNotBeNull();
        result.Id.ShouldBe(criado.Id);
        result.Nome.ShouldBe(criado.Nome);
    }

    [Fact]
    public async Task Dado_JogoInexistente_Quando_BuscarPorId_Entao_Retorna404()
    {
        var client = await TestAuthHelper.CreateAdminClientAsync(_fixture);

        var response = await client.GetAsync($"/jogos/{Guid.NewGuid()}", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    // ── PUT /jogos/{id} ──────────────────────────────────────────────────────

    [Fact]
    public async Task Dado_JogoExistente_Quando_AdminAtualizar_Entao_Retorna204EVerificaDados()
    {
        var client = await TestAuthHelper.CreateAdminClientAsync(_fixture);
        var criado = await CriarJogoAsync(client);

        var command = new UpdateGameCommand(
            criado.Id, "Nome Atualizado", "Descrição atualizada",
            249.90m, new DateTime(2015, 5, 19), ClassificacaoEtaria.Dezesseis);

        var response = await client.PutAsJsonAsync($"/jogos/{criado.Id}", command, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var atualizado = await (await client.GetAsync($"/jogos/{criado.Id}", TestContext.Current.CancellationToken))
            .ReadContentAsync<GameResponse>(TestContext.Current.CancellationToken);

        atualizado!.Nome.ShouldBe("Nome Atualizado");
        atualizado.Preco.ShouldBe(249.90m);
    }

    [Fact]
    public async Task Dado_JogoInexistente_Quando_Atualizar_Entao_Retorna404()
    {
        var client = await TestAuthHelper.CreateAdminClientAsync(_fixture);
        var command = new UpdateGameCommand(
            Guid.NewGuid(), "Nome", "Descrição",
            99m, DateTime.UtcNow, ClassificacaoEtaria.Livre);

        var response = await client.PutAsJsonAsync($"/jogos/{command.Id}", command, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Dado_UserSemPermissao_Quando_AtualizarJogo_Entao_Retorna403()
    {
        var adminClient = await TestAuthHelper.CreateAdminClientAsync(_fixture);
        var criado = await CriarJogoAsync(adminClient);

        var userClient = await TestAuthHelper.CreateUserClientAsync(_fixture);
        var command = new UpdateGameCommand(
            criado.Id, "Nome", "Descrição",
            99m, DateTime.UtcNow, ClassificacaoEtaria.Livre);

        var response = await userClient.PutAsJsonAsync($"/jogos/{criado.Id}", command, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    // ── PATCH /jogos/{id}/preco ───────────────────────────────────────────────

    [Fact]
    public async Task Dado_JogoExistente_Quando_AlterarPreco_Entao_Retorna204EVerificaPreco()
    {
        var client = await TestAuthHelper.CreateAdminClientAsync(_fixture);
        var criado = await CriarJogoAsync(client);

        var command = new UpdatePriceCommand(criado.Id, 149.90m);
        var response = await client.PatchAsJsonAsync($"/jogos/{criado.Id}/preco", command, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var atualizado = await (await client.GetAsync($"/jogos/{criado.Id}", TestContext.Current.CancellationToken))
            .ReadContentAsync<GameResponse>(TestContext.Current.CancellationToken);

        atualizado!.Preco.ShouldBe(149.90m);
    }

    [Fact]
    public async Task Dado_JogoInexistente_Quando_AlterarPreco_Entao_Retorna404()
    {
        var client = await TestAuthHelper.CreateAdminClientAsync(_fixture);
        var command = new UpdatePriceCommand(Guid.NewGuid(), 149.90m);

        var response = await client.PatchAsJsonAsync($"/jogos/{command.Id}/preco", command, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    // ── DELETE /jogos/{id} ───────────────────────────────────────────────────

    [Fact]
    public async Task Dado_JogoExistente_Quando_AdminDeletar_Entao_Retorna204()
    {
        var client = await TestAuthHelper.CreateAdminClientAsync(_fixture);
        var criado = await CriarJogoAsync(client);

        var response = await client.DeleteAsync($"/jogos/{criado.Id}", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Dado_JogoInexistente_Quando_Deletar_Entao_Retorna404()
    {
        var client = await TestAuthHelper.CreateAdminClientAsync(_fixture);

        var response = await client.DeleteAsync($"/jogos/{Guid.NewGuid()}", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Dado_UserSemPermissao_Quando_DeletarJogo_Entao_Retorna403()
    {
        var adminClient = await TestAuthHelper.CreateAdminClientAsync(_fixture);
        var criado = await CriarJogoAsync(adminClient);

        var userClient = await TestAuthHelper.CreateUserClientAsync(_fixture);
        var response = await userClient.DeleteAsync($"/jogos/{criado.Id}", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
