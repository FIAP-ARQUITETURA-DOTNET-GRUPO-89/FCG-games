using System.Net;
using System.Net.Http.Json;
using FgcGames.Application.Commands;
using FgcGames.Application.Responses;
using FgcGames.Domain.Entities;
using FgcGames.Domain.Enum;
using FgcGames.Domain.ValueObjects;
using FgcGames.IntegrationTests.Fixtures;
using Shouldly;

namespace FgcGames.IntegrationTests.Endpoints;

public class UpdateUserIntegrationTests(IntegrationTestFixture fixture) : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client = fixture.HttpClient;

    [Fact]
    public async Task Dado_DadosValidos_Quando_AtualizarPerfil_Entao_AtualizaComSucesso()
    {
        // ARRANGE
        var userId = Guid.Empty;
        await fixture.ExecuteDbContextAsync(async (context) =>
        {
            var usuario = new Usuario("Nome Antigo", new DateOnly(1990, 1, 1), Email.Create("perfil@teste.com"), Senha.FromHash("$2a$11$hashFakeParaTestes000000000000"), UserRole.User);
            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();
            userId = usuario.Id;
        });

        var command = new UpdateUserCommand(userId, "Novo Nome Generico", new DateOnly(1995, 10, 15));

        // ACT
        var response = await _client.PutAsJsonAsync($"/usuarios/users/{userId}", command, TestContext.Current.CancellationToken);

        // ASSERT
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UpdateUserResponse>(cancellationToken: TestContext.Current.CancellationToken);

        result.ShouldNotBeNull();
        result.Mensagem.ShouldBe("Perfil atualizado com sucesso!");

        await fixture.ExecuteDbContextAsync(async (context) =>
        {
            var userDb = await context.Usuarios.FindAsync(userId);

            userDb.ShouldNotBeNull();
            userDb.Nome.ShouldBe("Novo Nome Generico");
            userDb.DataNascimento.ShouldBe(new DateOnly(1995, 10, 15));
        });
    }

    [Fact]
    public async Task Dado_UsuarioInexistente_Quando_AtualizarPerfil_Entao_RetornaErroInterno()
    {
        // ARRANGE
        var idInexistente = Guid.NewGuid();
        var command = new UpdateUserCommand(idInexistente, "Nome Teste", DateOnly.FromDateTime(DateTime.Now).AddYears(-20));

        // ACT
        var response = await _client.PutAsJsonAsync($"/usuarios/users/{idInexistente}", command, TestContext.Current.CancellationToken);

        // ASSERT
        response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Dado_NovaRole_Quando_AtualizarRole_Entao_AlteraPermissaoNoBanco()
    {
        // ARRANGE
        var userId = Guid.Empty;
        await fixture.ExecuteDbContextAsync(async (context) =>
        {
            var usuario = new Usuario("User Role", new DateOnly(1990, 1, 1), Email.Create("role@teste.com"), Senha.FromHash("$2a$11$hashFakeParaTestes000000000000"), UserRole.User);
            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();
            userId = usuario.Id;
        });

        var novaRole = UserRole.Admin;
        var command = new UpdateUserRoleCommand(userId, novaRole);

        // ACT
        var response = await _client.PatchAsJsonAsync($"/usuarios/users/{userId}/role", command, TestContext.Current.CancellationToken);

        // ASSERT
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await fixture.ExecuteDbContextAsync(async (context) =>
        {
            var userDb = await context.Usuarios.FindAsync(userId);
            userDb.ShouldNotBeNull();
            userDb.Role.ShouldBe(UserRole.Admin);
        });
    }

    [Fact]
    public async Task Dado_SenhaValida_Quando_AtualizarSenha_Entao_GravaNovoHashNoBanco()
    {
        // ARRANGE
        var userId = Guid.Empty;
        await fixture.ExecuteDbContextAsync(async (context) =>
        {
            var usuario = new Usuario("User Senha", new DateOnly(1990, 1, 1), Email.Create("senha@teste.com"), Senha.FromHash("$2a$11$hashFakeParaTestes000000000000"), UserRole.User);
            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();
            userId = usuario.Id;
        });

        var novaSenhaRaw = "NovaSenhaForte@456";
        var command = new UpdatePasswordCommand(userId, novaSenhaRaw);

        // ACT
        var response = await _client.PatchAsJsonAsync($"/usuarios/users/{userId}/password", command, TestContext.Current.CancellationToken);

        // ASSERT
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await fixture.ExecuteDbContextAsync(async (context) =>
        {
            var userDb = await context.Usuarios.FindAsync(userId);
            userDb.ShouldNotBeNull();
            userDb.Senha.Hash.ShouldNotBe(novaSenhaRaw);
            BCrypt.Net.BCrypt.Verify(novaSenhaRaw, userDb.Senha.Hash).ShouldBeTrue();
        });
    }

    [Fact]
    public async Task Dado_SenhaInvalida_Quando_AtualizarSenha_Entao_RetornaBadRequest()
    {
        // ARRANGE
        var userId = Guid.Empty;
        await fixture.ExecuteDbContextAsync(async (context) =>
        {
            var usuario = new Usuario("User Senha Curta", new DateOnly(1990, 1, 1), Email.Create("short@teste.com"), Senha.FromHash("$2a$11$hashFakeParaTestes000000000000"), UserRole.User);
            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();
            userId = usuario.Id;
        });

        var command = new UpdatePasswordCommand(userId, "123");

        // ACT
        var response = await _client.PatchAsJsonAsync($"/usuarios/users/{userId}/password", command, TestContext.Current.CancellationToken);

        // ASSERT
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
