using System.Net;
using System.Net.Http.Json;
using FgcGames.Application.Responses;
using FgcGames.IntegrationTests.Fixtures;
using FgcGames.IntegrationTests.TestHelpers;
using Shouldly;

namespace FgcGames.IntegrationTests.Endpoints;

public class CRUDExampleIntegrationTests(IntegrationTestFixture fixture) : IClassFixture<IntegrationTestFixture>, IAsyncLifetime
{
    private readonly IntegrationTestFixture _fixture = fixture;
    private readonly HttpClient _client = fixture.HttpClient;

    public async ValueTask InitializeAsync()
    {
        await _fixture.ResetDatabaseAsync();
        _fixture.ResetAuth();
    }

    [Fact]
    public async Task Dado_DadosValidos_Quando_CriarTask_Entao_CriaComSucesso()
    {
        // Arrange
        await _fixture.AuthenticateAsAdminAsync();

        var request = new { title = $"Task {Guid.NewGuid()}" };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/crud-example/task-items",
            request,
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var result = await response.ReadContentAsync<CreateTaskItemExampleResponse>(TestContext.Current.CancellationToken);
        result.ShouldNotBeNull();
        result.Title.ShouldBe(request.title);
        result.IsCompleted.ShouldBeFalse();
        result.Id.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task Dado_TituloVazio_Quando_CriarTask_Entao_Retorna400()
    {
        // Arrange
        await _fixture.AuthenticateAsAdminAsync();

        var request = new { title = "" };

        // Act
        var response = await _client.PostAsJsonAsync("/crud-example/task-items", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var errors = await response.ReadValidationErrorsAsync(TestContext.Current.CancellationToken);
        errors.ShouldContainKey("Title");
        errors["Title"].ShouldContain("'Title' deve ser informado.");
    }

    [Fact]
    public async Task Dado_TituloMaiorQue100Caracteres_Quando_CriarTask_Entao_Retorna400()
    {
        await _fixture.AuthenticateAsAdminAsync();

        var request = new { title = new string('a', 101) };

        // Act
        var response = await _client.PostAsJsonAsync("/crud-example/task-items", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var errors = await response.ReadValidationErrorsAsync(TestContext.Current.CancellationToken);
        errors.ShouldContainKey("Title");
        errors["Title"].ShouldContain("'Title' deve ser menor ou igual a 100 caracteres. Você digitou 101 caracteres.");
    }

    [Fact]
    public async Task Dado_TituloDuplicado_Quando_CriarTask_Entao_Retorna409()
    {
        // Arrange
        await _fixture.AuthenticateAsAdminAsync();

        var title = $"Task {Guid.NewGuid()}";

        var request = new { title };

        var first = await _client.PostAsJsonAsync(
            "/crud-example/task-items",
            request,
            TestContext.Current.CancellationToken);

        first.StatusCode.ShouldBe(HttpStatusCode.Created);

        // Act
        var second = await _client.PostAsJsonAsync(
            "/crud-example/task-items",
            request,
            TestContext.Current.CancellationToken);

        // Assert
        second.StatusCode.ShouldBe(HttpStatusCode.Conflict);

        var problem = await second.ReadProblemDetailsAsync(TestContext.Current.CancellationToken);
        problem.Status.ShouldBe(409);
        problem.Title.ShouldBe("Conflict");
        problem.Detail.ShouldBe("Já existe uma task com esse título");
    }

    [Fact]
    public async Task Dado_SemToken_Quando_CriarTask_Entao_Retorna401()
    {
        // Arrange 
        _fixture.ResetAuth();

        var request = new { title = "Task sem auth" };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/crud-example/task-items",
            request,
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        var problem = await response.ReadProblemDetailsAsync(TestContext.Current.CancellationToken);
        problem.Status.ShouldBe(401);
        problem.Title.ShouldBe("Unauthorized");
        problem.Detail.ShouldBe("Credenciais de autenticação ausentes ou inválidas.");
    }


    [Fact]
    public async Task Dado_UserSemAutorizacao_Quando_CriarTask_Entao_Retorna403()
    {
        // Arrange
        await _fixture.AuthenticateAsUserAsync();

        var request = new { title = "Task proibida" };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/crud-example/task-items",
            request,
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);

        var problem = await response.ReadProblemDetailsAsync(TestContext.Current.CancellationToken);
        problem.Status.ShouldBe(403);
        problem.Title.ShouldBe("Forbidden");
        problem.Detail.ShouldBe("Você não tem permissão para acessar este recurso.");
    }

    [Fact]
    public async Task Dado_User_Quando_BuscarTask_Entao_PodeAcessar()
    {
        // Arrange
        await _fixture.AuthenticateAsAdminAsync();

        var create = await _client.PostAsJsonAsync("/crud-example/task-items",
            new { title = $"Task {Guid.NewGuid()}" },
            TestContext.Current.CancellationToken);

        var created = await create.ReadContentAsync<CreateTaskItemExampleResponse>(TestContext.Current.CancellationToken);

        await _fixture.AuthenticateAsUserAsync();

        // Act
        var response = await _client.GetAsync($"/crud-example/task-items/{created!.Id}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.ReadContentAsync<GetTaskItemByIdExampleResponse>(TestContext.Current.CancellationToken);
        result.ShouldNotBeNull();
        result.Title.Contains("Task");
        result.IsCompleted.ShouldBeFalse();
        result.Id.ShouldBeGreaterThan(0);
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
