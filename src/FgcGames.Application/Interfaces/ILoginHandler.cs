using FgcGames.Application.Commands;
using FgcGames.Application.Responses;

namespace FgcGames.Application.Interfaces;

public interface ILoginHandler
{
    Task<LoginResponse> Handle(LoginCommand command);
}
