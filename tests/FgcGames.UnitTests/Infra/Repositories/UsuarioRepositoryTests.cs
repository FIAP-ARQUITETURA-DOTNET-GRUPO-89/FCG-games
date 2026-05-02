using FgcGames.Domain.Entities;
using FgcGames.Domain.Enum;
using FgcGames.Domain.ValueObjects;
using FgcGames.Infra.Repositories;
using FgcGames.UnitTests.TestHelpers.Factories;
using Shouldly;

namespace FgcGames.UnitTests.Infra.Repositories;

public class UsuarioRepositoryTests
{
    public async Task Dado_EmailExistente_Quando_ObterPorEmail_Entao_DeveRetornarUsuario()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateContext();

        var admin = new Usuario(
            nome: "Admin",
            dataNascimento: new DateOnly(1990, 1, 1),
            email: new Email("teste@email.com"),
            senha: Senha.FromHash("Abc!1234"),
            userRole: UserRole.Admin
        );

        context.Usuarios.Add(admin);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new UsuarioRepository(context);

        // Act
        var usuario = await repository.ObterPorEmailAsync(admin.Email.Endereco);

        // Assert
        usuario.ShouldNotBeNull();
        usuario.Nome.ShouldBe(admin.Nome);
        usuario.DataNascimento.ShouldBe(admin.DataNascimento);
        usuario.Email.Endereco.ShouldBe(admin.Email.Endereco);
        usuario.Role.ShouldBe(admin.Role);
    }

    [Fact]
    public async Task Dado_EmailInexistente_Quando_ObterPorEmail_Entao_DeveRetornarNulo()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateContext();

        var email = "inexistente@email.com";

        var repository = new UsuarioRepository(context);

        // Act
        var usuario = await repository.ObterPorEmailAsync(email);

        // Assert
        usuario.ShouldBeNull();
    }

    [Fact]
    public async Task Dado_EmailExistente_ComDiferencaDeCase_Quando_ObterPorEmail_Entao_DeveRetornarUsuario()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateContext();

        var admin = new Usuario(
            nome: "Admin",
            dataNascimento: new DateOnly(1990, 1, 1),
            email: new Email("MAIUSCULO@EMAIL.COM"),
            senha: Senha.FromHash("Abc!1234"),
            userRole: UserRole.Admin
        );

        context.Usuarios.Add(admin);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new UsuarioRepository(context);
        var email = "maiusculo@email.com";

        // Act
        var usuario = await repository.ObterPorEmailAsync(email);

        // Assert
        usuario.ShouldNotBeNull();
        usuario.Nome.ShouldBe(admin.Nome);
        usuario.DataNascimento.ShouldBe(admin.DataNascimento);
        usuario.Email.Endereco.ShouldBe(admin.Email.Endereco);
        usuario.Role.ShouldBe(admin.Role);
    }

    [Fact]
    public async Task Dado_UsuarioValido_Quando_Adicionar_Entao_DevePersistir()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateContext();
        var repository = new UsuarioRepository(context);
        var usuario = CriarUsuario("Jonatas", "jonatas@email.com");

        // Act
        repository.Add(usuario);
        await repository.SaveChangesAsync();

        // Assert
        var result = await repository.GetByIdAsync(usuario.Id);
        result.ShouldNotBeNull();
        result!.Nome.ShouldBe("Jonatas");
        result.Email.Endereco.ShouldBe("jonatas@email.com");
    }

    [Fact]
    public async Task Dado_UsuarioExistente_Quando_Atualizar_Entao_DevePersistirAlteracoes()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateContext();
        var repository = new UsuarioRepository(context);
        var usuario = CriarUsuario("Nome Antigo", "user@email.com");

        repository.Add(usuario);
        await repository.SaveChangesAsync();

        // Act
        usuario.AtualizarPerfil("Nome Novo", new DateTime(1995, 5, 20));
        repository.Update(usuario);
        await repository.SaveChangesAsync();

        // Assert
        var result = await repository.GetByIdAsync(usuario.Id);
        result.ShouldNotBeNull();
        result!.Nome.ShouldBe("Nome Novo");
        result.DataNascimento.ShouldBe(new DateTime(1995, 5, 20));
    }

    [Fact]
    public async Task Dado_UsuarioExistente_Quando_Deletar_Entao_DeveRemover()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateContext();
        var repository = new UsuarioRepository(context);
        var usuario = CriarUsuario("Jonatas", "jonatas@email.com");

        repository.Add(usuario);
        await repository.SaveChangesAsync();

        // Act
        repository.Delete(usuario);
        await repository.SaveChangesAsync();

        // Assert
        var result = await repository.GetByIdAsync(usuario.Id);
        result.ShouldBeNull();
    }

    [Fact]
    public async Task Dado_MultiplosUsuarios_Quando_BuscarPaginado_Entao_DeveRetornarPaginaCorreta()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateContext();
        var repository = new UsuarioRepository(context);

        repository.Add(CriarUsuario("Ana", "ana@email.com"));
        repository.Add(CriarUsuario("Bruno", "bruno@email.com"));
        repository.Add(CriarUsuario("Carlos", "carlos@email.com"));
        await repository.SaveChangesAsync();

        // Act
        var result = await repository.GetPagedAsync(2, 2);

        // Assert
        result.Count.ShouldBe(1);
        result.First().Nome.ShouldBe("Carlos");
    }

    private static Usuario CriarUsuario(string nome, string email)
        => new(
            nome,
            new DateTime(1990, 1, 1),
            Email.Create(email),
            Senha.Create("Senha@123"),
            UserRole.User
        );
}
