namespace FgcGames.Application.Interfaces;

public interface ISenhaHasherService
{
    /// <summary>
    /// Gera um hash seguro a partir de uma senha.
    /// </summary>
    /// <param name="senha">Senha em texto.</param>
    /// <returns>Hash da senha gerado.</returns>
    string Hash(string senha);

    /// <summary>
    /// Verifica se uma senha em texto corresponde ao hash informado.
    /// </summary>
    /// <param name="senha">Senha em texto.</param>
    /// <param name="senhaHash">Hash previamente gerado.</param>
    /// <returns>True se a senha for válida; caso contrário, false.</returns>
    bool VerificarSenha(string senha, string senhaHash);
}
