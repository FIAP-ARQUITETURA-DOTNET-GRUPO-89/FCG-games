using System.Linq.Expressions;
using FgcGames.Application.Interfaces;
using FgcGames.Application.Queries;
using FgcGames.Application.Responses;
using FgcGames.Domain.Entities;
using FgcGames.Domain.Interfaces.Repositories;

namespace FgcGames.Application.Handlers;

public class GetAllUsersHandler(IUsuarioRepository repository) : IGetAllUsersHandler
{
    private readonly IUsuarioRepository _repository = repository;

    public async Task<PagedResponse<GetAllUsersResponse>> Handle(GetAllUsersQuery query)
    {
        Expression<Func<Usuario, bool>> filtro = u => u.Inativo == !query.Ativos;

        var totalUsers = await _repository.CountAsync(filtro);
        var users = await _repository.GetPagedAsync(
            query.Pagina,
            query.TamanhoPagina,
            filtro
        );

        var listaUsersResponse = users.Select(user => new GetAllUsersResponse(
            user.Id,
            user.Nome,
            user.Email.Endereco,
            user.DataNascimento,
            !user.Inativo
        ));

        var totalPaginas = (int)Math.Ceiling(totalUsers / (double)query.TamanhoPagina);

        return new PagedResponse<GetAllUsersResponse>(
            listaUsersResponse,
            query.Pagina,
            totalPaginas,
            totalUsers
        );
    }
}
