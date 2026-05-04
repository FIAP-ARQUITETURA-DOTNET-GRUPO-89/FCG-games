using System.Net;
using System.Net.Http.Json;
using FgcGames.Application.Commands;
using FgcGames.Application.Responses;
using FgcGames.IntegrationTests.Fixtures;
using FgcGames.Domain.Entities;
using FgcGames.Domain.ValueObjects;
using FgcGames.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace FgcGames.IntegrationTests.Endpoints.Users;

[Collection("IntegrationTests")]
public class CreateUserIntegrationTests(IntegrationTestFixture fixture) : IAsyncLifetime
{
    private readonly IntegrationTestFixture _fixture = fixture;
    private HttpClient _client = default!;

    public async ValueTask InitializeAsync()
    {
        await _fixture.ResetDatabaseAsync();
        _client = _fixture.HttpClient;
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task Dado_DadosValidos_Quando_CriarUsuario_Entao_SalvaNoBancoERetornarCreated()
    {
        // ARRANGE
        var command = new CreateUserCommand(
            "Novo Usuario Teste",
            "novo.item@gmail.com",
            new DateOnly(1998, 5, 12),
            "Senha12!"
        );

        // ACT
        var response = await _client.PostAsJsonAsync("/usuarios", command, TestContext.Current.CancellationToken);

        // ASSERT
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<CreateUserResponse>(cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);

        await _fixture.ExecuteDbContextAsync(async (context) =>
        {
            var usuarioDb = await context.Usuarios
                .FirstOrDefaultAsync(u => u.Email.Endereco == command.Email.ToLower());

            usuarioDb.ShouldNotBeNull();
            usuarioDb.Nome.ShouldBe(command.Nome);
            usuarioDb.Role.ShouldBe(UserRole.User);
            usuarioDb.Inativo.ShouldBeFalse();
            usuarioDb.Senha.Hash.ShouldNotBe(command.Senha);
        });
    }

    [Fact]
    public async Task Dado_EmailJaCadastrado_Quando_CriarUsuario_Entao_RetornaConflict()
    {
        // ARRANGE
        var emailRepetido = "email.repetido@gmail.com";

        await _fixture.ExecuteDbContextAsync(async (context) =>
        {
            context.Usuarios.RemoveRange(context.Usuarios);

            var usuarioExistente = new Usuario(
                "Usuario Ja Existente",
                new DateOnly(1990, 1, 1),
                Email.Create(emailRepetido),
                Senha.FromHash("SenhaForte@123"),
                UserRole.User
            );

            context.Usuarios.Add(usuarioExistente);
            await context.SaveChangesAsync();
        });

        var command = new CreateUserCommand
        (
            "Tentativa de Clone",
            emailRepetido,
            new DateOnly(2000, 5, 12),
            "Senha@12"
        );

        // ACT
        var response = await _client.PostAsJsonAsync("/usuarios", command, TestContext.Current.CancellationToken);

        // ASSERT
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Dado_SenhaCurta_Quando_CriarUsuario_Entao_RetornaBadRequest()
    {
        // ARRANGE
        var command = new CreateUserCommand
        (
            "Usuario Senha Fraca",
            "senha.curta@email.com",
            new DateOnly(1990, 5, 12),
            "123"
        );

        // ACT
        var response = await _client.PostAsJsonAsync("/usuarios", command, TestContext.Current.CancellationToken);

        // ASSERT
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
