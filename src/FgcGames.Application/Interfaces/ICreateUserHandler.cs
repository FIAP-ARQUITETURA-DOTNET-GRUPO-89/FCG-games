using FgcGames.Application.Commands;
using FgcGames.Application.Responses;

namespace FgcGames.Application.Interfaces;

public interface ICreateUserHandler
{
    Task<CreateUserResponse> Handle(CreateUserCommand command);
}
