using FgcGames.Domain.Entities;
using FgcGames.Domain.Enum;
using FgcGames.Domain.ValueObjects;
using FgcGames.Infra.Repositories;
using FgcGames.UnitTests.TestHelpers.Factories;
using Shouldly;

namespace FgcGames.UnitTests.Infra.Repositories;

public class UsuarioRepositoryTests
{
    [Fact]
    public async Task Dado_EmailExistente_Quando_ObterPorEmail_Entao_DeveRetornarUsuario()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateContext();
        var repository = new UsuarioRepository(context);
        var usuario = CriarUsuario("Jonatas", "jonatas@email.com");

        var admin = new Usuario(
            nome: "Admin",
            dataNascimento: new DateTime(1990, 1, 1),
            email: new Email("teste@email.com"),
            senha: Senha.FromHash("Abc!1234"),
            userRole: UserRole.Admin
        );

        context.Usuarios.Add(admin);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

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
        var repository = new UsuarioRepository(context);
        var usuario = CriarUsuario("Jonatas", "jonatas@email.com");

        repository.Add(usuario);
        await repository.SaveChangesAsync();

        // Act
        repository.Delete(usuario);
        await repository.SaveChangesAsync();

        var email = "inexistente@email.com";

    [Fact]
    public async Task Dado_EmailExistente_Quando_VerificarExistencia_Entao_DeveRetornarTrue()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateContext();
        var repository = new UsuarioRepository(context);

        repository.Add(CriarUsuario("Jonatas", "jonatas@email.com"));
        await repository.SaveChangesAsync();

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
        var repository = new UsuarioRepository(context);

        var admin = new Usuario(
            nome: "Admin",
            dataNascimento: new DateTime(1990, 1, 1),
            email: new Email("MAIUSCULO@EMAIL.COM"),
            senha: Senha.FromHash("Abc!1234"),
            userRole: UserRole.Admin
        );

        context.Usuarios.Add(admin);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

    [Fact]
    public async Task Dado_MultiplosUsuarios_Quando_BuscarPaginado_Entao_DeveRetornarPaginaCorreta()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateContext();
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

    private static Usuario CriarUsuario(string nome, string email)
        => new(
            nome,
            new DateTime(1990, 1, 1),
            Email.Create(email),
            Senha.Create("Senha@123"),
            UserRole.User
        );
}
