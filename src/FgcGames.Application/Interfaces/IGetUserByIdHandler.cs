using FgcGames.Application.Queries;
using FgcGames.Application.Responses;

namespace FgcGames.Application.Interfaces;

public interface IGetUserByIdHandler
{
    Task<GetUserByIdResponse?> Handle(GetUserByIdQuery query);
}
