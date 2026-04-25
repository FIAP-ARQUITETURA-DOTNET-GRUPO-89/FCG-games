using FgcGames.Application.Commands;
using FgcGames.Application.Responses;

namespace FgcGames.Application.Interfaces;

public interface IUpdatePasswordHandler
{
    Task<UpdatePasswordResponse> Handle(UpdatePasswordCommand command);
}
