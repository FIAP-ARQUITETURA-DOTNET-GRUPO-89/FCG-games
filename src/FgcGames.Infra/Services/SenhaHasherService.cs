using FgcGames.Application.Interfaces;

namespace FgcGames.Infra.Services;

public class SenhaHasherService : ISenhaHasherService
{
    public bool VerificarSenha(string senha, string senhaHash) => BCrypt.Net.BCrypt.Verify(senha, senhaHash);
}
