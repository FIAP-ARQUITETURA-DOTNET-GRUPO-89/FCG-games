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

namespace FgcGames.IntegrationTests.Endpoints;

public class CreateUserIntegrationTests(IntegrationTestFixture fixture) : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client = fixture.HttpClient;

    [Fact]
    public async Task Dado_DadosValidos_Quando_CriarUsuario_Entao_SalvaNoBancoERetornarCreated()
    {
        // ARRANGE
        var command = new CreateUserCommand(
            "Usuario Teste",
            "usuario.teste@gmail.com",
            new DateTime(1980, 10, 15),
            "Senha.Forte123!"
        );

        // ACT
        var response = await _client.PostAsJsonAsync("/usuarios/users", command, TestContext.Current.CancellationToken);

        // ASSERT
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<CreateUserResponse>(cancellationToken: TestContext.Current.CancellationToken);
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);

        await fixture.ExecuteDbContextAsync(async (context) =>
        {
            var usuarioDb = await context.Usuarios
                .FirstOrDefaultAsync(u => u.Email.Endereco == command.Email.ToLower());

            usuarioDb.ShouldNotBeNull();
            usuarioDb.Nome.ShouldBe(command.Nome);
            usuarioDb.Role.ShouldBe(UserRole.User);
            usuarioDb.Inativo.ShouldBeFalse();
            usuarioDb.Senha.Password.ShouldNotBe(command.Senha);
        });
    }

    [Fact]
    public async Task Dado_EmailJaCadastrado_Quando_CriarUsuario_Entao_RetornaBadRequest()
    {
        // ARRANGE
        var emailRepetido = "email.repetido@gmail.com";

        await fixture.ExecuteDbContextAsync(async (context) =>
        {
            context.Usuarios.RemoveRange(context.Usuarios);

            var usuarioExistente = new Usuario(
                "Usuario Ja Existente",
                new DateTime(1990, 1, 1),
                Email.Create(emailRepetido),
                Senha.Create("SenhaForte@123"),
                UserRole.User
            );

            context.Usuarios.Add(usuarioExistente);
            await context.SaveChangesAsync();
        });

        var command = new CreateUserCommand
        (
            "Tentativa de Clone",
            emailRepetido,
            new DateTime(2000, 1, 1),
            "Senha12345678"
        );

        // ACT
        var response = await _client.PostAsJsonAsync("/usuarios/users", command, TestContext.Current.CancellationToken);

        // ASSERT
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Dado_SenhaCurta_Quando_CriarUsuario_Entao_RetornaBadRequest()
    {
        // ARRANGE
        var command = new CreateUserCommand
        (
            "Usuario Senha Fraca",
            "senha.curta@email.com",
            new DateTime(1990, 1, 1),
            "123"
        );

        // ACT
        var response = await _client.PostAsJsonAsync("/usuarios/users", command, TestContext.Current.CancellationToken);

        // ASSERT
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
