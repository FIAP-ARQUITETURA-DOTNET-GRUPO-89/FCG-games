using FgcGames.Application.Commands;

namespace FgcGames.Application.Interfaces;

public interface IUpdateGameHandler
{
    Task Handle(UpdateGameCommand command);
}
