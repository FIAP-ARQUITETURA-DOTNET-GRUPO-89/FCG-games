using System.Net;
using System.Net.Http.Json;
using FgcGames.Application.Responses;
using FgcGames.IntegrationTests.Fixtures;
using FgcGames.IntegrationTests.TestHelpers;
using Shouldly;

namespace FgcGames.IntegrationTests.Endpoints;

public class AuthIntegrationTests(IntegrationTestFixture fixture) : IClassFixture<IntegrationTestFixture>, IAsyncLifetime
{
    private readonly IntegrationTestFixture _fixture = fixture;

    public async ValueTask InitializeAsync() => await _fixture.ResetDatabaseAsync();

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task Dado_CredenciaisValidas_Quando_Login_Entao_Retorna200ComToken()
    {
        // Arrange
        var client = TestAuthHelper.CreateAnonymousClient(_fixture);

        var request = new
        {
            email = "admin@fgcgames.com",
            senha = "Abc!1234"
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/auth",
            request,
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.ReadContentAsync<LoginResponse>(TestContext.Current.CancellationToken);
        result.ShouldNotBeNull();
        result.Token.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Dado_EmailInexistente_Quando_Login_Entao_Retorna401()
    {
        // Arrange
        var client = TestAuthHelper.CreateAnonymousClient(_fixture);

        var request = new
        {
            email = "naoexiste@fgcgames.com",
            senha = "Abc!1234"
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/auth",
            request,
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        var problem = await response.ReadProblemDetailsAsync(TestContext.Current.CancellationToken);
        problem.Status.ShouldBe(401);
        problem.Title.ShouldBe("Unauthorized");
        problem.Detail.ShouldBe("Credenciais inválidas.");
    }

    [Fact]
    public async Task Dado_SenhaInvalida_Quando_Login_Entao_Retorna401()
    {
        // Arrange
        var client = TestAuthHelper.CreateAnonymousClient(_fixture);

        var request = new
        {
            email = "admin@fgcgames.com",
            senha = "Abc!4321"
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/auth",
            request,
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        var problem = await response.ReadProblemDetailsAsync(TestContext.Current.CancellationToken);
        problem.Status.ShouldBe(401);
        problem.Title.ShouldBe("Unauthorized");
        problem.Detail.ShouldBe("Credenciais inválidas.");
    }

    [Fact]
    public async Task Dado_EmailVazio_Quando_Login_Entao_Retorna400()
    {
        // Arrange
        var client = TestAuthHelper.CreateAnonymousClient(_fixture);

        var request = new
        {
            email = "",
            senha = "Abc!1234"
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/auth",
            request,
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var errors = await response.ReadValidationErrorsAsync(TestContext.Current.CancellationToken);
        errors.ShouldContainKey("Email");
        errors["Email"].ShouldBe(
        [
            "'Email' deve ser informado.",
            "'Email' deve ter entre 1 e 254 caracteres. Você digitou 0 caracteres.",
            "O formato do email é inválido."
        ]);
    }

    [Fact]
    public async Task Dado_SenhaVazia_Quando_Login_Entao_Retorna400()
    {
        // Arrange
        var client = TestAuthHelper.CreateAnonymousClient(_fixture);

        var request = new
        {
            email = "admin@fgc.com",
            senha = ""
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/auth",
            request,
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var errors = await response.ReadValidationErrorsAsync(TestContext.Current.CancellationToken);
        errors.ShouldContainKey("Senha");
        errors["Senha"].ShouldBe(
        [
            "'Senha' deve ser informado.",
            "'Senha' deve ter exatamente 8 caracteres. Você digitou 0 caracteres.",
            "A senha deve conter pelo menos uma letra maiúscula.",
            "A senha deve conter pelo menos uma letra minúscula.",
            "A senha deve conter pelo menos um número.",
            "A senha deve conter pelo menos um caractere especial (!? *.)."
        ]);
    }

    [Fact]
    public async Task Dado_RequestInvalido_Quando_Login_Entao_Retorna400()
    {
        // Arrange
        var client = TestAuthHelper.CreateAnonymousClient(_fixture);

        var request = new { };

        // Act
        var response = await client.PostAsJsonAsync(
            "/auth",
            request,
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var errors = await response.ReadValidationErrorsAsync(TestContext.Current.CancellationToken);

        errors.ShouldContainKey("Email");
        errors.ShouldContainKey("Senha");

        errors["Email"].ShouldContain("'Email' deve ser informado.");
        errors["Senha"].ShouldContain("'Senha' deve ser informado.");
    }
}
