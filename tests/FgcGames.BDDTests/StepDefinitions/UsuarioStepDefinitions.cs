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
public class UsuarioStepDefinitions(ApiContext apiContext)
{
    [Given("estou autenticado como {string} com senha {string}")]
    public async Task EstouAutenticadoComo(string email, string senha)
    {
        apiContext.Client.DefaultRequestHeaders.Authorization = null;
        var response = await apiContext.Client.PostAsJsonAsync("/auth", new { email, senha });
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var token = json.GetProperty("token").GetString();
        apiContext.Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    [Given("não estou autenticado")]
    public void NaoEstouAutenticado()
        => apiContext.Client.DefaultRequestHeaders.Authorization = null;

    [Given("existe um usuário com email {string}")]
    public async Task ExisteUmUsuarioComEmail(string email)
    {
        var command = new CreateUserCommand("Usuario Teste", email, new DateOnly(1990, 1, 1), "Abc!1234");
        var response = await apiContext.Client.PostAsJsonAsync("/usuarios", command);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            if (json.TryGetProperty("id", out var id))
                apiContext.LastCreatedId = Guid.Parse(id.GetString()!);
        }
    }

    [Given("existe um usuário com nome {string} e email {string}")]
    public async Task ExisteUmUsuarioComNomeEEmail(string nome, string email)
    {
        var command = new CreateUserCommand(nome, email, new DateOnly(1990, 1, 1), "Abc!1234");
        var response = await apiContext.Client.PostAsJsonAsync("/usuarios", command);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            if (json.TryGetProperty("id", out var id))
                apiContext.LastCreatedId = Guid.Parse(id.GetString()!);
        }
    }

    [When("crio um usuário com nome {string}, email {string}, data de nascimento {string} e senha {string}")]
    public async Task CrioUmUsuario(string nome, string email, string dataNascimento, string senha)
    {
        var command = new CreateUserCommand(nome, email, DateOnly.Parse(dataNascimento), senha);
        apiContext.LastResponse = await apiContext.Client.PostAsJsonAsync("/usuarios", command);

        if (apiContext.LastResponse.IsSuccessStatusCode)
        {
            var json = await apiContext.LastResponse.Content.ReadFromJsonAsync<JsonElement>();
            if (json.TryGetProperty("id", out var id))
                apiContext.LastCreatedId = Guid.Parse(id.GetString()!);
        }
    }

    [When("busco o usuário pelo ID")]
    public async Task BuscoOUsuarioPeloId()
        => apiContext.LastResponse = await apiContext.Client.GetAsync($"/usuarios/{apiContext.LastCreatedId}");

    [When("busco o usuário com ID {string}")]
    public async Task BuscoOUsuarioComId(string id)
        => apiContext.LastResponse = await apiContext.Client.GetAsync($"/usuarios/{id}");

    [When("listo todos os usuários com página {int} e tamanho {int}")]
    public async Task ListoTodosOsUsuarios(int pagina, int tamanho)
        => apiContext.LastResponse = await apiContext.Client.GetAsync(
            $"/usuarios?pagina={pagina}&tamanhoPagina={tamanho}&ativos=true");

    [When("busco usuários pelo nome {string}")]
    public async Task BuscoUsuariosPeloNome(string nome)
        => apiContext.LastResponse = await apiContext.Client.GetAsync(
            $"/usuarios/busca?nome={nome}&pagina=1&tamanhoPagina=10");

    [When("atualizo o nome do usuário para {string} e data de nascimento {string}")]
    public async Task AtualizoONomeDoUsuario(string nome, string dataNascimento)
    {
        var command = new UpdateUserCommand(apiContext.LastCreatedId, nome, DateOnly.Parse(dataNascimento));
        apiContext.LastResponse = await apiContext.Client.PutAsJsonAsync(
            $"/usuarios/{apiContext.LastCreatedId}", command);
    }

    [When("atualizo a role do usuário para {string}")]
    public async Task AtualizoARoleDoUsuario(string role)
    {
        var userRole = Enum.Parse<UserRole>(role);
        var command = new UpdateUserRoleCommand(apiContext.LastCreatedId, userRole);
        apiContext.LastResponse = await apiContext.Client.PatchAsJsonAsync(
            $"/usuarios/{apiContext.LastCreatedId}/role", command);
    }

    [When("atualizo a senha do usuário para {string}")]
    public async Task AtualizoASenhaDoUsuario(string senha)
    {
        var command = new UpdatePasswordCommand(apiContext.LastCreatedId, senha);
        apiContext.LastResponse = await apiContext.Client.PatchAsJsonAsync(
            $"/usuarios/{apiContext.LastCreatedId}/password", command);
    }

    [When("deleto o usuário")]
    public async Task DeletoOUsuario()
        => apiContext.LastResponse = await apiContext.Client.DeleteAsync(
            $"/usuarios/{apiContext.LastCreatedId}");

    [When("deleto o usuário com ID {string}")]
    public async Task DeletoOUsuarioComId(string id)
        => apiContext.LastResponse = await apiContext.Client.DeleteAsync($"/usuarios/{id}");

    [Then("a resposta deve conter um ID de usuário válido")]
    public async Task ARespostaDeveConterIdDeUsuario()
    {
        var json = await apiContext.LastResponse!.Content.ReadFromJsonAsync<JsonElement>();
        json.TryGetProperty("id", out var id).ShouldBeTrue();
        Guid.Parse(id.GetString()!).ShouldNotBe(Guid.Empty);
    }

    [Then("a resposta deve conter os dados do usuário")]
    public async Task ARespostaDeveConterDadosDoUsuario()
    {
        var json = await apiContext.LastResponse!.Content.ReadFromJsonAsync<JsonElement>();
        json.TryGetProperty("id", out _).ShouldBeTrue();
        json.TryGetProperty("nome", out _).ShouldBeTrue();
    }

    [Then("a resposta deve conter uma lista paginada")]
    public async Task ARespostaDeveConterListaPaginada()
    {
        var json = await apiContext.LastResponse!.Content.ReadFromJsonAsync<JsonElement>();
        json.TryGetProperty("itens", out _).ShouldBeTrue();
        json.TryGetProperty("totalItens", out _).ShouldBeTrue();
    }
}
