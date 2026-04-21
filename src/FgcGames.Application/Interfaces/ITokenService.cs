namespace FgcGames.Application.Interfaces;

public interface ITokenService
{
    string GenerateJwtToken(string email, string role);
}
