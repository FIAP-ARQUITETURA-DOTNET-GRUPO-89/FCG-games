using System.Linq.Expressions;
using FgcGames.Application.Interfaces;
using FgcGames.Application.Queries;
using FgcGames.Application.Responses;
using FgcGames.Domain.Entities;
using FgcGames.Domain.Interfaces.Repositories;

namespace FgcGames.Application.Handlers;

public class GetUsersByNameHandler(IUsuarioRepository repository) : IGetUsersByNameHandler
{
    private readonly IUsuarioRepository _repository = repository;

    public async Task<PagedResponse<GetUsersByNameResponse>> Handle(GetUsersByNameQuery query)
    {
        if (string.IsNullOrWhiteSpace(query.Nome))
        {
            return new PagedResponse<GetUsersByNameResponse>(
                [],
                query.Pagina, 0, 0
            );
        }

        Expression<Func<Usuario, bool>> filtro = u => u.Nome.Contains(query.Nome) && !u.Inativo;

        var totalUsers = await _repository.CountAsync(filtro);
        var users = await _repository.GetPagedAsync(
            query.Pagina,
            query.TamanhoPagina,
            filtro
        );

        var listaUsersResponse = users.Select(user => new GetUsersByNameResponse(
            user.Id,
            user.Nome,
            user.DataNascimento
        ));

        var totalPaginas = (int)Math.Ceiling(totalUsers / (double)query.TamanhoPagina);

        return new PagedResponse<GetUsersByNameResponse>(
            listaUsersResponse,
            query.Pagina,
            totalPaginas,
            totalUsers
        );
    }
}
