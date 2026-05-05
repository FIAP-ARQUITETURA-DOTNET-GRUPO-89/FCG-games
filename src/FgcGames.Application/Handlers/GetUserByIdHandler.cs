using FgcGames.Application.Interfaces;
using FgcGames.Application.Queries;
using FgcGames.Application.Responses;
using FgcGames.Domain.Interfaces.Repositories;

namespace FgcGames.Application.Handlers;

public class GetUserByIdHandler(IUsuarioRepository repository) : IGetUserByIdHandler
{
    private readonly IUsuarioRepository _repository = repository;

    public async Task<GetUserByIdResponse?> Handle(GetUserByIdQuery query)
    {
        var user = await _repository.GetByIdAsync(query.Id);

        if (user is null)
        {
            return null;
        }

        return new GetUserByIdResponse(
            user.Id,
            user.Nome,
            user.Email.Endereco,
            user.DataNascimento,
            !user.Inativo
        );
    }
}
