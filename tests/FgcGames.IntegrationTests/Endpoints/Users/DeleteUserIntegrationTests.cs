using System.Net;
using System.Net.Http.Json;
using FgcGames.Application.Responses;
using FgcGames.Domain.Entities;
using FgcGames.Domain.Enum;
using FgcGames.Domain.ValueObjects;
using FgcGames.IntegrationTests.Fixtures;
using FgcGames.IntegrationTests.TestHelpers;
using Shouldly;

namespace FgcGames.IntegrationTests.Endpoints.Users;

[Collection("IntegrationTests")]
public class DeleteUserIntegrationTests(IntegrationTestFixture fixture) : IAsyncLifetime
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
    public async Task Dado_UsuarioAtivoNoBanco_Quando_InativarUsuario_Entao_MudaStatusParaInativoERetornaOk()
    {
        // ARRANGE
        var userId = Guid.Empty;

        await _fixture.ExecuteDbContextAsync(async (context) =>
        {
            context.Usuarios.RemoveRange(context.Usuarios);

            var usuario = new Usuario(
                "Usuario Comum",
                new DateOnly(1990, 1, 1),
                Email.Create("delete@teste.com"),
                Senha.FromHash("Senha@123"),
                UserRole.User
            );

            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();
            userId = usuario.Id;
        });

        // ACT
        var response = await _client.DeleteAsync($"/usuarios/{userId}", TestContext.Current.CancellationToken);

        // ASSERT
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<DeleteUserResponse>(cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldNotBeNull();
        result.Inativo.ShouldBeTrue();

        await _fixture.ExecuteDbContextAsync(async (context) =>
        {
            var userDb = await context.Usuarios.FindAsync(userId);
            userDb.ShouldNotBeNull();
            userDb.Inativo.ShouldBeTrue();
        });
    }

    [Fact]
    public async Task Dado_UsuarioInexistente_Quando_InativarUsuario_Entao_RetornaNotFound()
    {
        // ARRANGE
        var idInexistente = Guid.NewGuid();

        // ACT
        var response = await _client.DeleteAsync($"/usuarios/{idInexistente}", TestContext.Current.CancellationToken);

        // ASSERT
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
