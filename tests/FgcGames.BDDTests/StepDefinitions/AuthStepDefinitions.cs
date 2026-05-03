using System.Net.Http.Json;
using System.Text.Json;
using FgcGames.BDDTests.Support;
using Microsoft.AspNetCore.Mvc;
using Reqnroll;
using Shouldly;

namespace FgcGames.BDDTests.StepDefinitions;

[Binding]
public class AuthStepDefinitions(ApiContext apiContext)
{
    [When("faço login com email {string} e senha {string}")]
    public async Task FacoLoginComEmailESenha(string email, string senha)
        => apiContext.LastResponse = await apiContext.Client.PostAsJsonAsync("/auth", new { email, senha });

    [When("faço login sem informar credenciais")]
    public async Task FacoLoginSemCredenciais()
        => apiContext.LastResponse = await apiContext.Client.PostAsJsonAsync("/auth", new { });

    [Then("a resposta deve ter status {int}")]
    public void ARespostaDeveTerStatus(int statusCode)
        => ((int)apiContext.LastResponse!.StatusCode).ShouldBe(statusCode);

    [Then("a resposta deve conter um token JWT válido")]
    public async Task ARespostaDeveConterToken()
    {
        var json = await apiContext.LastResponse!.Content.ReadFromJsonAsync<JsonElement>();
        json.TryGetProperty("token", out var token).ShouldBeTrue();
        token.GetString().ShouldNotBeNullOrWhiteSpace();
    }

    [Then("o detalhe do erro deve ser {string}")]
    public async Task ODetalheDoErroDeveSer(string detalhe)
    {
        var problem = await apiContext.LastResponse!.Content.ReadFromJsonAsync<ProblemDetails>();
        problem!.Detail.ShouldBe(detalhe);
    }

    [Then("os erros de validação devem conter o campo {string}")]
    public async Task OsErrosDeValidacaoDevemConterOCampo(string campo)
    {
        var json = await apiContext.LastResponse!.Content.ReadFromJsonAsync<JsonElement>();
        json.TryGetProperty("errors", out var errors).ShouldBeTrue();
        errors.TryGetProperty(campo, out _).ShouldBeTrue();
    }
}
