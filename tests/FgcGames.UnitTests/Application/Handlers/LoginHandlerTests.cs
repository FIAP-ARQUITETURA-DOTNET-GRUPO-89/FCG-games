using FgcGames.Application.Commands;
using FgcGames.Application.Handlers;
using FgcGames.Application.Interfaces;
using FgcGames.Domain.Entities;
using FgcGames.Domain.Enum;
using FgcGames.Domain.Interfaces.Repositories;
using FgcGames.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace FgcGames.UnitTests.Application.Handlers;

public class LoginHandlerTests
{
    private readonly ILogger<LoginHandler> _logger;
    private readonly IUsuarioRepository _repository;
    private readonly ITokenService _tokenService;
    private readonly ISenhaHasherService _senhaHasher;
    private readonly LoginHandler _sut;

    public LoginHandlerTests()
    {
        _logger = Substitute.For<ILogger<LoginHandler>>();
        _repository = Substitute.For<IUsuarioRepository>();
        _tokenService = Substitute.For<ITokenService>();
        _senhaHasher = Substitute.For<ISenhaHasherService>();

        _sut = new LoginHandler(_logger, _repository, _tokenService, _senhaHasher);
    }

    [Fact]
    public async Task Dado_CredenciaisValidas_Quando_Login_Entao_RetornaToken()
    {
        // Arrange
        var command = new LoginCommand("user@fgc.com", "123456");
        var usuario = CriarUsuario();

        _repository.ObterPorEmailAsync(command.Email)
            .Returns(usuario);

        _senhaHasher.VerificarSenha(command.Senha, usuario.Senha.Hash)
            .Returns(true);

        _tokenService.GenerateJwtToken(usuario.Email.Endereco, usuario.Role.ToString())
            .Returns("token-valido");

        // Act
        var result = await _sut.Handle(command);

        // Assert
        result.ShouldNotBeNull();
        result.Token.ShouldBe("token-valido");

        await _repository.Received(1).ObterPorEmailAsync(command.Email);
        _senhaHasher.Received(1).VerificarSenha(command.Senha, usuario.Senha.Hash);
        _tokenService.Received(1).GenerateJwtToken(usuario.Email.Endereco, usuario.Role.ToString());
    }

    [Fact]
    public async Task Dado_UsuarioNaoExiste_Quando_Login_Entao_LancaUnauthorized()
    {
        // Arrange
        var command = new LoginCommand("naoexiste@fgc.com", "123456");

        _repository.ObterPorEmailAsync(command.Email)
            .Returns((Usuario?)null);

        // Act
        var exception = await Should.ThrowAsync<UnauthorizedAccessException>(() =>
            _sut.Handle(command));

        // Assert
        exception.Message.ShouldBe("Credenciais inválidas.");

        await _repository.Received(1).ObterPorEmailAsync(command.Email);

        _senhaHasher.DidNotReceiveWithAnyArgs().VerificarSenha(default!, default!);
        _tokenService.DidNotReceiveWithAnyArgs().GenerateJwtToken(default!, default!);
    }

    [Fact]
    public async Task Dado_UsuarioInativo_Quando_Login_Entao_LancaUnauthorizedSemRevelarMotivo()
    {
        // Arrange
        var command = new LoginCommand("user@fgc.com", "123456");
        var usuario = CriarUsuario();
        usuario.Inativar();

        _repository.ObterPorEmailAsync(command.Email)
            .Returns(usuario);

        _senhaHasher.VerificarSenha(command.Senha, usuario.Senha.Hash)
            .Returns(true);

        // Act
        var exception = await Should.ThrowAsync<UnauthorizedAccessException>(() =>
            _sut.Handle(command));

        // Assert
        exception.Message.ShouldBe("Credenciais inválidas.");

        await _repository.Received(1).ObterPorEmailAsync(command.Email);

        _senhaHasher.DidNotReceiveWithAnyArgs().VerificarSenha(default!, default!);
    }

    [Fact]
    public async Task Dado_SenhaInvalida_Quando_Login_Entao_LancaUnauthorized()
    {
        // Arrange
        var command = new LoginCommand("user@fgc.com", "senha-errada");
        var usuario = CriarUsuario();

        _repository.ObterPorEmailAsync(command.Email)
            .Returns(usuario);

        _senhaHasher.VerificarSenha(command.Senha, usuario.Senha.Hash)
            .Returns(false);

        // Act
        var exception = await Should.ThrowAsync<UnauthorizedAccessException>(() =>
            _sut.Handle(command));

        // Assert
        exception.Message.ShouldBe("Credenciais inválidas.");

        await _repository.Received(1).ObterPorEmailAsync(command.Email);
        _senhaHasher.Received(1).VerificarSenha(command.Senha, usuario.Senha.Hash);

        _tokenService.DidNotReceiveWithAnyArgs().GenerateJwtToken(default!, default!);
    }


    [Theory]
    [InlineData("admin@fgc.com", UserRole.Admin)]
    [InlineData("user@fgc.com", UserRole.User)]
    public async Task Dado_LoginValido_Quando_GerarToken_Entao_UsaEmailERoleCorretos(
        string email,
        UserRole role)
    {
        // Arrange
        var command = new LoginCommand(email, "123456");
        var usuario = CriarUsuario(email: email, role: role);

        _repository.ObterPorEmailAsync(command.Email)
            .Returns(usuario);

        _senhaHasher.VerificarSenha(command.Senha, usuario.Senha.Hash)
            .Returns(true);

        _tokenService.GenerateJwtToken(Arg.Any<string>(), Arg.Any<string>())
            .Returns("token");

        // Act
        await _sut.Handle(command);

        // Assert
        _tokenService.Received(1).GenerateJwtToken(email, role.ToString());
    }

    private static Usuario CriarUsuario(string email = "user@fgc.com", string hash = "hash", UserRole role = UserRole.Admin)
        => new(
            nome: "Usuário Teste",
            dataNascimento: new DateTime(2000, 1, 1),
            email: new Email(email),
            senha: Senha.FromHash(hash),
            userRole: role
        );
}
