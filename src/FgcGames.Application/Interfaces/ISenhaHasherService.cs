namespace FgcGames.Application.Interfaces;

public interface ISenhaHasherService
{
    bool VerificarSenha(string senha, string senhaHash);
}
