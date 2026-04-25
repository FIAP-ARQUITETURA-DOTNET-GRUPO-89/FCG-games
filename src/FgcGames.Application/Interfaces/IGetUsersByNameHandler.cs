using FgcGames.Application.Queries;
using FgcGames.Application.Responses;

namespace FgcGames.Application.Interfaces;

public interface IGetUsersByNameHandler
{
    Task<PagedResponse<GetUsersByNameResponse>> Handle(GetUsersByNameQuery query);
}
