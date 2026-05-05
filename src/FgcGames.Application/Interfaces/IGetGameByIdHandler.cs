using FgcGames.Application.Queries;
using FgcGames.Application.Responses;

namespace FgcGames.Application.Interfaces;

public interface IGetGameByIdHandler
{
    Task<GameResponse> Handle(GetGameByIdQuery query);
}
