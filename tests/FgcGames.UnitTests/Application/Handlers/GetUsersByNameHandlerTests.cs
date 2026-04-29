using FgcGames.Application.Handlers;
using FgcGames.Application.Queries;
using FgcGames.Domain.Entities;
using FgcGames.Domain.Enum;
using FgcGames.Domain.Interfaces.Repositories;
using FgcGames.Domain.ValueObjects;
using NSubstitute;
using Shouldly;

namespace FgcGames.UnitTests.Application.Handlers;

public class GetUsersByNameHandlerTests
{
    private readonly IUsuarioRepository _repository;
    private readonly GetUsersByNameHandler _sut;

    public GetUsersByNameHandlerTests()
    {
        _repository = Substitute.For<IUsuarioRepository>();
        _sut = new GetUsersByNameHandler(_repository);
    }

    [Fact]
    public async Task Dado_NomeVazio_Quando_Buscar_Entao_DeveRetornarListaVazia()
    {
        var query = new GetUsersByNameQuery("", 1, 10);

        var result = await _sut.Handle(query);

        result.Itens.ShouldBeEmpty();
        result.TotalItens.ShouldBe(0);
        result.TotalPaginas.ShouldBe(0);

        await _repository.DidNotReceiveWithAnyArgs().CountAsync(default!);
        await _repository.DidNotReceiveWithAnyArgs().GetPagedAsync(default, default, default!);
    }

    [Fact]
    public async Task Dado_NomeValido_Quando_Buscar_Entao_DeveRetornarPaginado()
    {
        var query = new GetUsersByNameQuery("Jo", 1, 10);

        var users = new List<Usuario>
        {
            CriarUsuario("João"),
            CriarUsuario("Jonatas")
        };

        _repository.CountAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Usuario, bool>>>()).Returns(2);
        _repository.GetPagedAsync(query.Pagina, query.TamanhoPagina, Arg.Any<System.Linq.Expressions.Expression<Func<Usuario, bool>>>())
            .Returns(users);

        var result = await _sut.Handle(query);

        result.Itens.Count().ShouldBe(2);
        result.PaginaAtual.ShouldBe(1);
        result.TotalItens.ShouldBe(2);
        result.TotalPaginas.ShouldBe(1);

        await _repository.Received(1).CountAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Usuario, bool>>>());
        await _repository.Received(1).GetPagedAsync(query.Pagina, query.TamanhoPagina, Arg.Any<System.Linq.Expressions.Expression<Func<Usuario, bool>>>());
    }

    private static Usuario CriarUsuario(string nome)
        => new(nome, new DateTime(1990, 1, 1), Email.Create($"{nome.ToLower()}@email.com"), Senha.FromHash("Senha@123"), UserRole.User);
}
