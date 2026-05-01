using FgcGames.Application.Queries;
using FgcGames.Application.Responses;

namespace FgcGames.Application.Interfaces;

public interface IGetAllUsersHandler
{
    Task<PagedResponse<GetAllUsersResponse>> Handle(GetAllUsersQuery query);
}
