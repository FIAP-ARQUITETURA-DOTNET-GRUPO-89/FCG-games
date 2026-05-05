using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FgcGames.Application.Commands;
using FgcGames.Application.Handlers;
using FgcGames.Domain.Entities;
using FgcGames.Domain.Enum;
using FgcGames.Domain.Interfaces.Repositories;
using FgcGames.Domain.ValueObjects;
using FgcGames.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Shouldly;

namespace FgcGames.UnitTests.Application.Handlers;

public class UpdatePasswordHandlerTests
{
    private readonly Microsoft.Extensions.Logging.ILogger<UpdatePasswordHandler> _logger;
    private readonly IUsuarioRepository _repository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UpdatePasswordHandler _sut;

    public UpdatePasswordHandlerTests()
    {
        _logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<UpdatePasswordHandler>>();
        _repository = Substitute.For<IUsuarioRepository>();
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();

        _sut = new UpdatePasswordHandler(_logger, _repository, _httpContextAccessor);
    }

    [Fact]
    public async Task Dado_UsuarioExistente_Quando_AtualizarSenha_Entao_DeveAtualizarComHash()
    {
        // Arrange
        var usuario = CriarUsuario();
        var command = new UpdatePasswordCommand(usuario.Id, "Senha@123");
        _repository.GetByIdAsync(command.Id).Returns(usuario);

        SimularUsuarioAutenticado(usuario.Email.Endereco);

        // Act
        var result = await _sut.Handle(command);

        // Assert
        result.ShouldNotBeNull();
        result.Mensagem.ShouldBe("Senha atualizada com sucesso!");
        usuario.Senha.Hash.ShouldNotBe(command.Password);
        usuario.Senha.Hash.StartsWith("$2").ShouldBeTrue();
        await _repository.Received(1).GetByIdAsync(command.Id);
        _repository.Received(1).Update(usuario);
        await _repository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Dado_UsuarioInexistente_Quando_AtualizarSenha_Entao_DeveLancarNotFoundException()
    {
        // Arrange
        var command = new UpdatePasswordCommand(Guid.NewGuid(), "Senha@123");
        _repository.GetByIdAsync(command.Id).Returns((Usuario?)null);
        SimularUsuarioAutenticado("qualquer@email.com");

        // Act
        await Should.ThrowAsync<NotFoundException>(async () => await _sut.Handle(command));

        // Assert
        _repository.DidNotReceiveWithAnyArgs().Update(default!);
        await _repository.DidNotReceive().SaveChangesAsync();
    }

    [Fact]
    public async Task Dado_UsuarioTentandoAlterarSenhaDeOutro_Entao_DeveLancarHttpRequestException()
    {
        // Arrange
        var usuarioNoBanco = CriarUsuario();
        var command = new UpdatePasswordCommand(usuarioNoBanco.Id, "Senha@123");
        _repository.GetByIdAsync(command.Id).Returns(usuarioNoBanco);
        SimularUsuarioAutenticado("invasor@email.com");

        // Act
        var task = _sut.Handle(command);

        // Assert
        await Should.ThrowAsync<HttpRequestException>(() => task);
    }

    [Fact]
    public async Task Dado_SenhaInvalida_Quando_AtualizarSenha_Entao_DeveLancarArgumentException()
    {
        // Arrange
        var usuario = CriarUsuario();
        var command = new UpdatePasswordCommand(usuario.Id, "123");
        _repository.GetByIdAsync(command.Id).Returns(usuario);

        SimularUsuarioAutenticado(usuario.Email.Endereco);

        // Act 
        await Should.ThrowAsync<ArgumentException>(async () => await _sut.Handle(command));

        // Assert
        _repository.DidNotReceiveWithAnyArgs().Update(default!);
        await _repository.DidNotReceive().SaveChangesAsync();
    }

    private void SimularUsuarioAutenticado(string email)
    {
        var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, email) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = principal };
        _httpContextAccessor.HttpContext.Returns(httpContext);
    }

    private static Usuario CriarUsuario()
        => new("Nome", new DateOnly(1990, 1, 1), Email.Create("user@email.com"), Senha.FromHash("hash_antigo"), UserRole.User);
}
