using FgcGames.Domain.Entities;
using FgcGames.Domain.Enum;
using FgcGames.Domain.ValueObjects;
using Shouldly;

namespace FgcGames.UnitTests.Domain.Entities;

public class UsuarioTests
{
    [Fact]
    public void Dado_DadosValidos_Quando_CriarUsuario_Entao_DeveInicializarCorretamente()
    {
        var nome = "Jonatas";
        var dataNascimento = new DateTime(1990, 1, 1);
        var email = Email.Create("jonatas@email.com");
        var senha = Senha.FromHash("12345678");

        var usuario = new Usuario(nome, DateOnly.FromDateTime(dataNascimento), email, senha, UserRole.User);

        usuario.ShouldNotBeNull();
        usuario.Nome.ShouldBe(nome);
        usuario.DataNascimento.ShouldBe(DateOnly.FromDateTime(dataNascimento));
        usuario.Email.ShouldBe(email);
        usuario.Senha.ShouldBe(senha);
        usuario.Role.ShouldBe(UserRole.User);
        usuario.Inativo.ShouldBeFalse();
    }

    [Fact]
    public void Dado_NomeValidoEDataValida_Quando_AtualizarPerfil_Entao_DeveAtualizarDados()
    {
        var usuario = new Usuario(
            "Nome Antigo",
            new DateOnly(1990, 1, 1),
            Email.Create("user@email.com"),
            Senha.FromHash("12345678"),
            UserRole.User);

        var novoNome = "Nome Novo";
        var novaDataNascimento = new DateOnly(1995, 5, 20);

        usuario.AtualizarPerfil(novoNome, novaDataNascimento);

        usuario.Nome.ShouldBe(novoNome);
        usuario.DataNascimento.ShouldBe(novaDataNascimento);
    }

    [Fact]
    public void Dado_NomeVazio_Quando_AtualizarPerfil_Entao_DeveLancarException()
    {
        var usuario = new Usuario(
            "Nome",
            new DateOnly(1990, 1, 1),
            Email.Create("user@email.com"),
            Senha.FromHash("12345678"),
            UserRole.User);

        var exception = Should.Throw<ArgumentException>(() => usuario.AtualizarPerfil("", new DateOnly(1995, 1, 1)));

        exception.Message.ShouldBe("O nome não pode estar vazio.");
    }

    [Fact]
    public void Dado_DataNascimentoFutura_Quando_AtualizarPerfil_Entao_DeveLancarException()
    {
        var usuario = new Usuario(
            "Nome",
            new DateOnly(1990, 1, 1),
            Email.Create("user@email.com"),
            Senha.FromHash("12345678"),
            UserRole.User);

        var dataFutura = DateOnly.FromDateTime(DateTime.Now.AddDays(1));

        var exception = Should.Throw<ArgumentException>(() => usuario.AtualizarPerfil("Nome Novo", dataFutura));

        exception.Message.ShouldBe("A data de nascimento não pode ser uma data futura.");
    }

    [Fact]
    public void Dado_NovaSenhaValida_Quando_AlterarSenha_Entao_DeveAtualizarSenha()
    {
        var usuario = new Usuario(
            "Nome",
            new DateOnly(1990, 1, 1),
            Email.Create("user@email.com"),
            Senha.FromHash("12345678"),
            UserRole.User);

        var novaSenha = Senha.FromHash("87654321");

        usuario.AlterarSenha(novaSenha);

        usuario.Senha.ShouldBe(novaSenha);
    }

    [Fact]
    public void Dado_UsuarioAtivo_Quando_Inativar_Entao_DeveMarcarComoInativo()
    {
        var usuario = new Usuario(
            "Nome",
            new DateOnly(1990, 1, 1),
            Email.Create("user@email.com"),
            Senha.FromHash("12345678"),
            UserRole.User);

        usuario.Inativar();

        usuario.Inativo.ShouldBeTrue();
    }

    [Fact]
    public void Dado_UsuarioAdmin_Quando_VerificarEhAdmin_Entao_DeveRetornarTrue()
    {
        var usuario = new Usuario(
            "Admin",
            new DateOnly(1990, 1, 1),
            Email.Create("admin@email.com"),
            Senha.FromHash("12345678"),
            UserRole.Admin);

        var ehAdmin = usuario.EhAdmin();

        ehAdmin.ShouldBeTrue();
    }

    [Fact]
    public void Dado_UsuarioComum_Quando_VerificarEhAdmin_Entao_DeveRetornarFalse()
    {
        var usuario = new Usuario(
            "User",
            new DateOnly(1990, 1, 1),
            Email.Create("user@email.com"),
            Senha.FromHash("12345678"),
            UserRole.User);

        var ehAdmin = usuario.EhAdmin();

        ehAdmin.ShouldBeFalse();
    }
}
