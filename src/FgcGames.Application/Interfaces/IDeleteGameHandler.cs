using FgcGames.Application.Commands;

namespace FgcGames.Application.Interfaces;

public interface IDeleteGameHandler
{
    Task Handle(DeleteGameCommand command);
}
