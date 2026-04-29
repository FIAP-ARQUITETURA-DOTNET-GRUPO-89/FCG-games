using FgcGames.Application.Commands;
using FgcGames.Application.Responses;

namespace FgcGames.Application.Interfaces;

public interface IDeleteUserHandler
{
    Task<DeleteUserResponse> Handle(DeleteUserCommand command);
}
