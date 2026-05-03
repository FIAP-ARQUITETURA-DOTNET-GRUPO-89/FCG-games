using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FgcGames.Application.Commands;
using FgcGames.BDDTests.Support;
using FgcGames.Domain.Enum;
using Reqnroll;
using Shouldly;

namespace FgcGames.BDDTests.StepDefinitions;

[Binding]
public class JogoStepDefinitions(ApiContext apiContext)
{
    [Given("existe um jogo cadastrado com nome {string}")]
    public async Task ExisteUmJogoCadastrado(string nome)
    {
        var savedAuth = apiContext.Client.DefaultRequestHeaders.Authorization;

        var loginResponse = await apiContext.Client.PostAsJsonAsync("/auth",
            new { email = "admin@fgcgames.com", senha = "Abc!1234" });

        if (loginResponse.IsSuccessStatusCode)
        {
            var json = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
            var token = json.GetProperty("token").GetString();
            apiContext.Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        var command = new CreateGameCommand(
            nome, "Descrição padrão para teste.", 99.90m,
            new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            ClassificacaoEtaria.Livre
        );

        var response = await apiContext.Client.PostAsJsonAsync("/jogos", command);
        apiContext.Client.DefaultRequestHeaders.Authorization = savedAuth;

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            if (json.TryGetProperty("id", out var id))
                apiContext.LastCreatedId = Guid.Parse(id.GetString()!);
        }
    }

    [When("crio um jogo com nome {string}, descrição {string}, preço {decimal}, lançamento {string} e classificação {int}")]
    public async Task CrioUmJogo(string nome, string descricao, decimal preco, string lancamento, int classificacao)
    {
        var command = new CreateGameCommand(
            nome, descricao, preco,
            DateTime.Parse(lancamento).ToUniversalTime(),
            (ClassificacaoEtaria)classificacao
        );

        apiContext.LastResponse = await apiContext.Client.PostAsJsonAsync("/jogos", command);

        if (apiContext.LastResponse.IsSuccessStatusCode)
        {
            var json = await apiContext.LastResponse.Content.ReadFromJsonAsync<JsonElement>();
            if (json.TryGetProperty("id", out var id))
                apiContext.LastCreatedId = Guid.Parse(id.GetString()!);
        }
    }

    [When("listo todos os jogos")]
    public async Task ListoTodosOsJogos()
        => apiContext.LastResponse = await apiContext.Client.GetAsync("/jogos");

    [When("busco o jogo pelo ID")]
    public async Task BuscoOJogoPeloId()
        => apiContext.LastResponse = await apiContext.Client.GetAsync($"/jogos/{apiContext.LastCreatedId}");

    [When("busco o jogo com ID {string}")]
    public async Task BuscoOJogoComId(string id)
        => apiContext.LastResponse = await apiContext.Client.GetAsync($"/jogos/{id}");

    [When("atualizo o jogo com nome {string}, descrição {string}, preço {decimal}, lançamento {string} e classificação {int}")]
    public async Task AtualizoOJogo(string nome, string descricao, decimal preco, string lancamento, int classificacao)
    {
        var command = new UpdateGameCommand(
            apiContext.LastCreatedId, nome, descricao, preco,
            DateTime.Parse(lancamento).ToUniversalTime(),
            (ClassificacaoEtaria)classificacao
        );

        apiContext.LastResponse = await apiContext.Client.PutAsJsonAsync(
            $"/jogos/{apiContext.LastCreatedId}", command);
    }

    [When("atualizo o jogo com ID {string} com nome {string}")]
    public async Task AtualizoOJogoComId(string id, string nome)
    {
        var command = new UpdateGameCommand(
            Guid.Parse(id), nome, "Descrição", 99m,
            DateTime.UtcNow, ClassificacaoEtaria.Livre
        );

        apiContext.LastResponse = await apiContext.Client.PutAsJsonAsync($"/jogos/{id}", command);
    }

    [When("altero o preço do jogo para {decimal}")]
    public async Task AlteroOPrecoDoJogo(decimal novoPreco)
    {
        var command = new UpdatePriceCommand(apiContext.LastCreatedId, novoPreco);
        apiContext.LastResponse = await apiContext.Client.PatchAsJsonAsync(
            $"/jogos/{apiContext.LastCreatedId}/preco", command);
    }

    [When("altero o preço do jogo com ID {string} para {decimal}")]
    public async Task AlteroOPrecoDoJogoComId(string id, decimal novoPreco)
    {
        var command = new UpdatePriceCommand(Guid.Parse(id), novoPreco);
        apiContext.LastResponse = await apiContext.Client.PatchAsJsonAsync($"/jogos/{id}/preco", command);
    }

    [When("deleto o jogo")]
    public async Task DeletoOJogo()
        => apiContext.LastResponse = await apiContext.Client.DeleteAsync($"/jogos/{apiContext.LastCreatedId}");

    [When("deleto o jogo com ID {string}")]
    public async Task DeletoOJogoComId(string id)
        => apiContext.LastResponse = await apiContext.Client.DeleteAsync($"/jogos/{id}");

    [Then("a resposta deve conter um ID de jogo válido")]
    public async Task ARespostaDeveConterIdDeJogo()
    {
        var json = await apiContext.LastResponse!.Content.ReadFromJsonAsync<JsonElement>();
        json.TryGetProperty("id", out var id).ShouldBeTrue();
        Guid.Parse(id.GetString()!).ShouldNotBe(Guid.Empty);
    }

    [Then("a resposta deve conter uma lista de jogos")]
    public async Task ARespostaDeveConterListaDeJogos()
    {
        var json = await apiContext.LastResponse!.Content.ReadFromJsonAsync<JsonElement>();
        json.ValueKind.ShouldBe(JsonValueKind.Array);
    }

    [Then("a resposta deve conter os dados do jogo")]
    public async Task ARespostaDeveConterDadosDoJogo()
    {
        var json = await apiContext.LastResponse!.Content.ReadFromJsonAsync<JsonElement>();
        json.TryGetProperty("id", out _).ShouldBeTrue();
        json.TryGetProperty("nome", out _).ShouldBeTrue();
    }
}
