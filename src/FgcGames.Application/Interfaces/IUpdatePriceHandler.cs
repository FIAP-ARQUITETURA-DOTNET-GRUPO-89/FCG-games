using FgcGames.Application.Commands;

namespace FgcGames.Application.Interfaces
{
    public interface IUpdatePriceHandler
    {
        Task Handle(UpdatePriceCommand command);
    }
}
