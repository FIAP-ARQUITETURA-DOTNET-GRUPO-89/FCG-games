using FgcGames.Application.Commands;
using FgcGames.Application.Responses;

namespace FgcGames.Application.Interfaces;

public interface IUpdateUserHandler
{
    Task<UpdateUserResponse> Handle(UpdateUserCommand command);
}
