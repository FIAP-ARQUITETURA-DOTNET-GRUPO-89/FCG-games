using FgcGames.Application.Interfaces;

namespace FgcGames.Infra.Services;

public class SenhaHasherService : ISenhaHasherService
{
    public string Hash(string senha) =>
        BCrypt.Net.BCrypt.HashPassword(senha);

    public bool VerificarSenha(string senha, string senhaHash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(senha, senhaHash);
        }
        catch
        {
            return false;
        }
    }
}
