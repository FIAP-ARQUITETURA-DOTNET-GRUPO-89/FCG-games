using System.Net;
using System.Net.Http.Json;
using FgcGames.Application.Commands;
using FgcGames.Application.Responses;
using FgcGames.Domain.Entities;
using FgcGames.Domain.Enum;
using FgcGames.Domain.ValueObjects;
using FgcGames.IntegrationTests.Fixtures;
using FgcGames.IntegrationTests.TestHelpers;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace FgcGames.IntegrationTests.Endpoints;

[Collection("IntegrationTests")]
public class UsuarioIntegrationTests(IntegrationTestFixture fixture) : IAsyncLifetime
{
    private readonly IntegrationTestFixture _fixture = fixture;

    public async ValueTask InitializeAsync() => await _fixture.ResetDatabaseAsync();

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    // ── POST /usuarios ─────────────────────────────

    [Fact]
    public async Task Dado_DadosValidos_Quando_CriarUsuario_Entao_SalvaNoBancoERetornarCreated()
    {
        // Arrange
        var client = TestAuthHelper.CreateAnonymousClient(_fixture);

        var command = new CreateUserCommand(
            "Novo Usuario Teste",
            "novo.item@gmail.com",
            new DateOnly(1998, 5, 12),
            "Senha12!"
        );

        // Act
        var response = await client.PostAsJsonAsync("/usuarios", command, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var result = await response.ReadContentAsync<CreateUserResponse>(TestContext.Current.CancellationToken);
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);

        await _fixture.ExecuteDbContextAsync(async context =>
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
        // Arrange
        var client = TestAuthHelper.CreateAnonymousClient(_fixture);
        var email = "email.repetido@gmail.com";

        await _fixture.ExecuteDbContextAsync(async context =>
        {
            context.Usuarios.RemoveRange(context.Usuarios);

            context.Usuarios.Add(new Usuario(
                "Usuario Existente",
                new DateOnly(1990, 1, 1),
                Email.Create(email),
                Senha.FromHash("Senha@123"),
                UserRole.User));

            await context.SaveChangesAsync();
        });

        var command = new CreateUserCommand(
            "Clone",
            email,
            new DateOnly(2000, 5, 12),
            "Senha@12"
        );

        // Act
        var response = await client.PostAsJsonAsync("/usuarios", command, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);

        var problem = await response.ReadProblemDetailsAsync(TestContext.Current.CancellationToken);
        problem.Status.ShouldBe(409);
        problem.Title.ShouldBe("Conflict");
        problem.Detail.ShouldBe("Já existe um usuário com esse email");
    }

    [Fact]
    public async Task Dado_EmailVazio_Quando_CriarUsuario_Entao_RetornaBadRequest()
    {
        // Arrange
        var client = TestAuthHelper.CreateAnonymousClient(_fixture);

        var command = new CreateUserCommand(
            "Usuario",
            "",
            new DateOnly(1990, 5, 12),
            "123"
        );

        // Act
        var response = await client.PostAsJsonAsync("/usuarios", command, TestContext.Current.CancellationToken);

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
    public async Task Dado_SenhaVazia_Quando_CriarUsuario_Entao_RetornaBadRequest()
    {
        // Arrange
        var client = TestAuthHelper.CreateAnonymousClient(_fixture);

        var command = new CreateUserCommand(
            "Usuario",
            "teste@email.com",
            new DateOnly(1990, 5, 12),
            ""
        );

        // Act
        var response = await client.PostAsJsonAsync("/usuarios", command, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var errors = await response.ReadValidationErrorsAsync(TestContext.Current.CancellationToken);
        errors.ShouldContainKey("Senha");
        errors["Senha"].ShouldBe(
        [
            "'Senha' deve ser informado.",
            "'Senha' deve ter entre 8 e 12 caracteres. Você digitou 0 caracteres.",
            "A senha deve conter pelo menos uma letra maiúscula.",
            "A senha deve conter pelo menos uma letra minúscula.",
            "A senha deve conter pelo menos um número.",
            "A senha deve conter pelo menos um caractere especial. Exemplos permitidos: ! ? * . @ # $ % &"
        ]);
    }

    [Fact]
    public async Task Dado_SenhaCurta_Quando_CriarUsuario_Entao_RetornaBadRequest()
    {
        // Arrange
        var client = TestAuthHelper.CreateAnonymousClient(_fixture); 

        var command = new CreateUserCommand(
            "Usuario",
            "teste@email.com",
            new DateOnly(1990, 5, 12),
            "123"
        );

        // Act
        var response = await client.PostAsJsonAsync("/usuarios", command, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var errors = await response.ReadValidationErrorsAsync(TestContext.Current.CancellationToken);
        errors.ShouldContainKey("Senha");
        errors["Senha"].ShouldBe(
        [
            "'Senha' deve ter entre 8 e 12 caracteres. Você digitou 3 caracteres.",
            "A senha deve conter pelo menos uma letra maiúscula.",
            "A senha deve conter pelo menos uma letra minúscula.",
             "A senha deve conter pelo menos um caractere especial. Exemplos permitidos: ! ? * . @ # $ % &"
        ]);
    }

    // ── DELETE /usuarios ───────────────────────────

    [Fact]
    public async Task Dado_UsuarioAtivo_Quando_Deletar_Entao_InativaERetornaOk()
    {
        // Arrange
        var client = await TestAuthHelper.CreateAdminClientAsync(_fixture);
        Guid userId = Guid.Empty;

        await _fixture.ExecuteDbContextAsync(async context =>
        {
            context.Usuarios.RemoveRange(context.Usuarios);

            var usuario = new Usuario(
                "Usuario",
                new DateOnly(1990, 1, 1),
                Email.Create("delete@teste.com"),
                Senha.FromHash("Senha@123"),
                UserRole.User);

            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();
            userId = usuario.Id;
        });

        // Act
        var response = await client.DeleteAsync($"/usuarios/{userId}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.ReadContentAsync<DeleteUserResponse>(TestContext.Current.CancellationToken);
        result.ShouldNotBeNull();
        result.Inativo.ShouldBeTrue();

        await _fixture.ExecuteDbContextAsync(async context =>
        {
            var user = await context.Usuarios.FindAsync(userId);

            user.ShouldNotBeNull();
            user.Inativo.ShouldBeTrue();
        });
    }

    [Fact]
    public async Task Dado_IdInexistente_Quando_Deletar_Entao_RetornaNotFound()
    {
        // Arrange
        var client = await TestAuthHelper.CreateAdminClientAsync(_fixture);
        var id = Guid.NewGuid();

        // Act
        var response = await client.DeleteAsync($"/usuarios/{id}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var problem = await response.ReadProblemDetailsAsync(TestContext.Current.CancellationToken);
        problem.Status.ShouldBe(404);
        problem.Title.ShouldBe("Not Found");
        problem.Detail!.Contains("Usuário não encontrado.");
    }

    [Fact]
    public async Task Dado_UserComum_Quando_DeletarUsuario_Entao_Retorna403()
    {
        // Arrange
        var client = await TestAuthHelper.CreateUserClientAsync(_fixture);

        // Act
        var response = await client.DeleteAsync($"/usuarios/{Guid.NewGuid()}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);

        var problem = await response.ReadProblemDetailsAsync(TestContext.Current.CancellationToken);
        problem.Status.ShouldBe(403);
        problem.Title.ShouldBe("Forbidden");
        problem.Detail.ShouldBe("Você não tem permissão para acessar este recurso.");
    }

    // ── GET /usuarios ──────────────────────────────

    [Fact]
    public async Task Dado_NomeExistente_Quando_Buscar_Entao_RetornaListaCorreta()
    {
        // Arrange
        var client = await TestAuthHelper.CreateAdminClientAsync(_fixture);
        var termo = "Oliveira";

        await _fixture.ExecuteDbContextAsync(async context =>
        {
            context.Usuarios.RemoveRange(context.Usuarios);

            await context.Usuarios.AddRangeAsync(
                new Usuario("Marcos Oliveira", new DateOnly(1990, 1, 1), Email.Create("1@email.com"), Senha.FromHash("123"), UserRole.User),
                new Usuario("Fernanda Oliveira", new DateOnly(1990, 1, 1), Email.Create("2@email.com"), Senha.FromHash("123"), UserRole.User)
            );

            await context.SaveChangesAsync();
        });

        // Act
        var response = await client.GetAsync($"/usuarios/busca?nome={termo}&pagina=1&tamanhoPagina=10", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.ReadContentAsync<PagedResponse<GetUsersByNameResponse>>(TestContext.Current.CancellationToken);
        result.ShouldNotBeNull();
        result.Itens.Count().ShouldBe(2);
        result.TotalItens.ShouldBe(2);

    }

    [Fact]
    public async Task Dado_UserComum_Quando_BuscarPorId_Entao_Retorna403()
    {
        // Arrange
        var client = await TestAuthHelper.CreateUserClientAsync(_fixture);

        // Act
        var response = await client.GetAsync($"/usuarios/{Guid.NewGuid()}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);

        var problem = await response.ReadProblemDetailsAsync(TestContext.Current.CancellationToken);
        problem.Status.ShouldBe(403);
        problem.Title.ShouldBe("Forbidden");
        problem.Detail.ShouldBe("Você não tem permissão para acessar este recurso.");
    }

    [Fact]
    public async Task Dado_SemAutenticacao_Quando_BuscarPorId_Entao_Retorna401()
    {
        // Arrange
        var client = TestAuthHelper.CreateAnonymousClient(_fixture);

        // Act
        var response = await client.GetAsync($"/usuarios/{Guid.NewGuid()}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        var problem = await response.ReadProblemDetailsAsync(TestContext.Current.CancellationToken);
        problem.Status.ShouldBe(401);
        problem.Title.ShouldBe("Unauthorized");
        problem.Detail.ShouldBe("Credenciais de autenticação ausentes ou inválidas.");
    }

    // ── PUT /usuarios ──────────────────────────────

    [Fact]
    public async Task Dado_DadosValidos_Quando_AtualizarPerfil_Entao_RetornaOk()
    {
        // Arrange
        var client = await TestAuthHelper.CreateAdminClientAsync(_fixture);
        Guid userId = Guid.Empty;

        await _fixture.ExecuteDbContextAsync(async context =>
        {
            var user = new Usuario(
                "Antigo",
                new DateOnly(1990, 1, 1),
                Email.Create("perfil@teste.com"),
                Senha.FromHash("123"),
                UserRole.User);

            context.Usuarios.Add(user);
            await context.SaveChangesAsync();
            userId = user.Id;
        });

        var command = new UpdateUserCommand(userId, "Novo Nome", new DateOnly(1995, 10, 15));

        // Act
        var response = await client.PutAsJsonAsync($"/usuarios/{userId}", command, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _fixture.ExecuteDbContextAsync(async context =>
        {
            var user = await context.Usuarios.FindAsync(userId);
            user!.Nome.ShouldBe("Novo Nome");
        });
    }

    [Fact]
    public async Task Dado_SenhaValida_Quando_AtualizarSenha_Entao_AtualizaHash()
    {
        // Arrange
        var client = await TestAuthHelper.CreateUserClientAsync(_fixture);
        var email = "user@fgcgames.com";
        Guid userId = Guid.Empty;

        await _fixture.ExecuteDbContextAsync(async context =>
        {
            var user = await context.Usuarios.FirstAsync(u => u.Email.Endereco == email);
            userId = user.Id;
        });

        var novaSenha = "NovaSenha@2";

        // Act
        var response = await client.PatchAsJsonAsync(
            $"/usuarios/{userId}/password",
            new UpdatePasswordCommand(userId, novaSenha),
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _fixture.ExecuteDbContextAsync(async context =>
        {
            var user = await context.Usuarios.FindAsync(userId);
            BCrypt.Net.BCrypt.Verify(novaSenha, user!.Senha.Hash).ShouldBeTrue();
        });
    }
}
