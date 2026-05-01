using FgcGames.Application.Commands;
using FgcGames.Application.Responses;

namespace FgcGames.Application.Interfaces;

public interface IUpdateUserRoleHandler
{
    Task<UpdateUserRoleResponse> Handle(UpdateUserRoleCommand command);
}
