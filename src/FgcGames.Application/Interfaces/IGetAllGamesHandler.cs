using FgcGames.Application.Queries;
using FgcGames.Application.Responses;

namespace FgcGames.Application.Interfaces
{
    public interface IGetAllGamesHandler
    {
        Task<IEnumerable<GameResponse>> Handle(GetAllGamesQuery query);
    }
}
