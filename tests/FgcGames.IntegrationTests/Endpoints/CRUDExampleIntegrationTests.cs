using System.Net;
using System.Net.Http.Json;
using FgcGames.Application.Responses;
using FgcGames.IntegrationTests.TestHelpers;
using Shouldly;

namespace FgcGames.IntegrationTests.Endpoints;

public class CRUDExampleIntegrationTests(IntegrationTestFixture fixture): IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client = fixture.HttpClient;

    [Fact]
    public async Task Dado_DadosValidos_Quando_CriarTask_Entao_CriaComSucesso()
    {
        // Arrange
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
        // Arrange
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
}
