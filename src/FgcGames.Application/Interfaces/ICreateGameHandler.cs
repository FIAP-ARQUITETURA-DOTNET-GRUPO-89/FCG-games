using FgcGames.Application.Commands;
using FgcGames.Application.Responses;

namespace FgcGames.Application.Interfaces;

public interface ICreateGameHandler
{
    Task<GameResponse> Handle(CreateGameCommand command);
}
