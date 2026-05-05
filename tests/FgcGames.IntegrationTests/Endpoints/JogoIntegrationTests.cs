using System.Net;
using System.Net.Http.Json;
using FgcGames.Application.Commands;
using FgcGames.Application.Responses;
using FgcGames.Domain.Entities;
using FgcGames.Domain.Enum;
using FgcGames.IntegrationTests.Fixtures;
using FgcGames.IntegrationTests.TestHelpers;
using Shouldly;

namespace FgcGames.IntegrationTests.Endpoints;

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
        new DateTime(2015, 5, 19),
        ClassificacaoEtaria.Dezoito
    );

    private async Task<Guid> CriarJogoAsync(string nome = "Jogo")
    {
        Guid jogoId = Guid.Empty;

        await _fixture.ExecuteDbContextAsync(async context =>
        {
            var jogo = new Jogo(
                new DateTime(2015, 5, 19),
                nome,
                "Desc",
                100,
                ClassificacaoEtaria.Dezoito);

            context.Jogos.Add(jogo);
            await context.SaveChangesAsync();

            jogoId = jogo.Id;
        });

        return jogoId;
    }

    // ── POST ─────────────────────────────────────────────

    [Fact]
    public async Task Dado_DadosValidos_Quando_AdminCriarJogo_Entao_Retorna201()
    {
        // Arrange
        var client = await TestAuthHelper.CreateAdminClientAsync(_fixture);

        // Act
        var response = await client.PostAsJsonAsync("/jogos", NovoJogo(), TestContext.Current.CancellationToken);

        // Assert
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
        // Arrange
        var client = await TestAuthHelper.CreateUserClientAsync(_fixture);

        // Act
        var response = await client.PostAsJsonAsync("/jogos", NovoJogo(), TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);

        var problem = await response.ReadProblemDetailsAsync(TestContext.Current.CancellationToken);
        problem.Status.ShouldBe(403);
        problem.Title.ShouldBe("Forbidden");
        problem.Detail.ShouldBe("Você não tem permissão para acessar este recurso.");
    }

    [Fact]
    public async Task Dado_SemAutenticacao_Quando_CriarJogo_Entao_Retorna401()
    {
        // Arrange
        var client = TestAuthHelper.CreateAnonymousClient(_fixture);

        // Act
        var response = await client.PostAsJsonAsync("/jogos", NovoJogo(), TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        var problem = await response.ReadProblemDetailsAsync(TestContext.Current.CancellationToken);
        problem.Status.ShouldBe(401);
        problem.Title.ShouldBe("Unauthorized");
        problem.Detail.ShouldBe("Credenciais de autenticação ausentes ou inválidas.");
    }

    // ── GET LIST ─────────────────────────────────────────

    [Fact]
    public async Task Dado_JogosExistentes_Quando_AdminListar_Entao_Retorna200ComLista()
    {
        // Arrange
        var client = await TestAuthHelper.CreateAdminClientAsync(_fixture);

        await _fixture.ExecuteDbContextAsync(async context =>
        {
            context.Jogos.AddRange(
                new Jogo(new DateTime(2015, 5, 19), "Jogo A", "Desc", 100, ClassificacaoEtaria.Dezoito),
                new Jogo(new DateTime(2015, 5, 19), "Jogo B", "Desc", 200, ClassificacaoEtaria.Dezoito)
            );

            await context.SaveChangesAsync();
        });

        // Act
        var response = await client.GetAsync("/jogos", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.ReadContentAsync<PagedResponse<GameResponse>>(TestContext.Current.CancellationToken);
        result.ShouldNotBeNull();
        result.Itens.Count().ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task Dado_UserAutenticado_Quando_ListarJogos_Entao_Retorna200()
    {
        // Arrange
        await _fixture.ExecuteDbContextAsync(async context =>
        {
            context.Jogos.AddRange(
                new Jogo(new DateTime(2015, 5, 19), "Jogo A", "Desc", 100, ClassificacaoEtaria.Dezoito),
                new Jogo(new DateTime(2015, 5, 19), "Jogo B", "Desc", 200, ClassificacaoEtaria.Dezoito)
            );

            await context.SaveChangesAsync();
        });

        var userClient = await TestAuthHelper.CreateUserClientAsync(_fixture);

        // Act
        var response = await userClient.GetAsync("/jogos", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.ReadContentAsync<PagedResponse<GameResponse>>(TestContext.Current.CancellationToken);
        result.ShouldNotBeNull();
        result.Itens.Count().ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task Dado_SemAutenticacao_Quando_ListarJogos_Entao_Retorna401()
    {
        // Arrange
        var client = TestAuthHelper.CreateAnonymousClient(_fixture);

        // Act
        var response = await client.GetAsync("/jogos", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        var problem = await response.ReadProblemDetailsAsync(TestContext.Current.CancellationToken);
        problem.Status.ShouldBe(401);
        problem.Title.ShouldBe("Unauthorized");
        problem.Detail.ShouldBe("Credenciais de autenticação ausentes ou inválidas.");
    }

    // ── GET BY ID ────────────────────────────────────────

    [Fact]
    public async Task Dado_JogoExistente_Quando_BuscarPorId_Entao_Retorna200()
    {
        // Arrange
        var client = await TestAuthHelper.CreateAdminClientAsync(_fixture);

        var jogoId = await CriarJogoAsync();

        // Act
        var response = await client.GetAsync($"/jogos/{jogoId}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.ReadContentAsync<GameResponse>(TestContext.Current.CancellationToken);
        result.ShouldNotBeNull();

        result.Id.ShouldBe(jogoId);
    }

    [Fact]
    public async Task Dado_JogoInexistente_Quando_BuscarPorId_Entao_Retorna404()
    {
        // Arrange
        var client = await TestAuthHelper.CreateAdminClientAsync(_fixture);

        // Act
        var response = await client.GetAsync($"/jogos/{Guid.NewGuid()}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var problem = await response.ReadProblemDetailsAsync(TestContext.Current.CancellationToken);
        problem.Status.ShouldBe(404);
        problem.Title.ShouldBe("Not Found");
        problem.Detail!.Contains("Jogo não encontrado.");
    }

    // ── PUT ──────────────────────────────────────────────

    [Fact]
    public async Task Dado_JogoExistente_Quando_AdminAtualizar_Entao_Retorna204EAtualizaDados()
    {
        // Arrange
        var client = await TestAuthHelper.CreateAdminClientAsync(_fixture);

        var jogoId = await CriarJogoAsync();

        var command = new UpdateGameCommand(
            jogoId,
            "Nome Atualizado",
            "Descrição atualizada",
            249.90m,
            new DateTime(2015, 5, 19),
            ClassificacaoEtaria.Dezesseis);

        // Act
        var response = await client.PutAsJsonAsync($"/jogos/{jogoId}", command, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var getResponse = await client.GetAsync($"/jogos/{jogoId}", TestContext.Current.CancellationToken);
        var atualizado = await getResponse.ReadContentAsync<GameResponse>(TestContext.Current.CancellationToken);

        atualizado.ShouldNotBeNull();
        atualizado.Nome.ShouldBe("Nome Atualizado");
        atualizado.Preco.ShouldBe(249.90m);
    }

    [Fact]
    public async Task Dado_JogoInexistente_Quando_Atualizar_Entao_Retorna404()
    {
        // Arrange
        var client = await TestAuthHelper.CreateAdminClientAsync(_fixture);

        var command = new UpdateGameCommand(
            Guid.NewGuid(),
            "Nome",
            "Descrição",
            99m,
            new DateTime(2015, 5, 19),
            ClassificacaoEtaria.Livre);

        // Act
        var response = await client.PutAsJsonAsync($"/jogos/{command.Id}", command, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var problem = await response.ReadProblemDetailsAsync(TestContext.Current.CancellationToken);
        problem.Status.ShouldBe(404);
        problem.Title.ShouldBe("Not Found");
        problem.Detail!.Contains("Jogo não encontrado.");
    }

    [Fact]
    public async Task Dado_UserSemPermissao_Quando_AtualizarJogo_Entao_Retorna403()
    {
        // Arrange
        var adminClient = await TestAuthHelper.CreateAdminClientAsync(_fixture);

        var jogoId = await CriarJogoAsync();

        var userClient = await TestAuthHelper.CreateUserClientAsync(_fixture);

        var command = new UpdateGameCommand(
            jogoId,
            "Nome",
            "Descrição",
            99m,
            new DateTime(2015, 5, 19),
            ClassificacaoEtaria.Livre);

        // Act
        var response = await userClient.PutAsJsonAsync($"/jogos/{jogoId}", command, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);

        var problem = await response.ReadProblemDetailsAsync(TestContext.Current.CancellationToken);
        problem.Status.ShouldBe(403);
        problem.Title.ShouldBe("Forbidden");
        problem.Detail.ShouldBe("Você não tem permissão para acessar este recurso.");
    }

    // ── PATCH ────────────────────────────────────────────

    [Fact]
    public async Task Dado_JogoExistente_Quando_AlterarPreco_Entao_Retorna204EAtualizaPreco()
    {
        // Arrange
        var client = await TestAuthHelper.CreateAdminClientAsync(_fixture);

        var jogoId = await CriarJogoAsync();

        var command = new UpdatePriceCommand(jogoId, 149.90m);

        // Act
        var response = await client.PatchAsJsonAsync($"/jogos/{jogoId}/preco", command, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var getResponse = await client.GetAsync($"/jogos/{jogoId}", TestContext.Current.CancellationToken);
        var atualizado = await getResponse.ReadContentAsync<GameResponse>(TestContext.Current.CancellationToken);

        atualizado.ShouldNotBeNull();
        atualizado.Preco.ShouldBe(149.90m);
    }

    [Fact]
    public async Task Dado_JogoInexistente_Quando_AlterarPreco_Entao_Retorna404()
    {
        // Arrange
        var client = await TestAuthHelper.CreateAdminClientAsync(_fixture);
        var command = new UpdatePriceCommand(Guid.NewGuid(), 149.90m);

        // Act
        var response = await client.PatchAsJsonAsync($"/jogos/{command.Id}/preco", command, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var problem = await response.ReadProblemDetailsAsync(TestContext.Current.CancellationToken);
        problem.Status.ShouldBe(404);
        problem.Title.ShouldBe("Not Found");
        problem.Detail!.Contains("Jogo não encontrado.");
    }

    // ── DELETE ───────────────────────────────────────────

    [Fact]
    public async Task Dado_JogoExistente_Quando_AdminDeletar_Entao_Retorna204()
    {
        // Arrange
        var client = await TestAuthHelper.CreateAdminClientAsync(_fixture);

        var jogoId = await CriarJogoAsync();

        // Act
        var response = await client.DeleteAsync($"/jogos/{jogoId}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Dado_JogoInexistente_Quando_Deletar_Entao_Retorna404()
    {
        // Arrange
        var client = await TestAuthHelper.CreateAdminClientAsync(_fixture);

        // Act
        var response = await client.DeleteAsync($"/jogos/{Guid.NewGuid()}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var problem = await response.ReadProblemDetailsAsync(TestContext.Current.CancellationToken);
        problem.Status.ShouldBe(404);
        problem.Title.ShouldBe("Not Found");
        problem.Detail!.Contains("Jogo não encontrado.");
    }

    [Fact]
    public async Task Dado_UserSemPermissao_Quando_DeletarJogo_Entao_Retorna403()
    {
        // Arrange
        var jogoId = await CriarJogoAsync();

        var userClient = await TestAuthHelper.CreateUserClientAsync(_fixture);

        // Act
        var response = await userClient.DeleteAsync($"/jogos/{jogoId}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);

        var problem = await response.ReadProblemDetailsAsync(TestContext.Current.CancellationToken);
        problem.Status.ShouldBe(403);
        problem.Title.ShouldBe("Forbidden");
        problem.Detail.ShouldBe("Você não tem permissão para acessar este recurso.");
    }
}
