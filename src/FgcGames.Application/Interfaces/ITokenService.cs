namespace FgcGames.Application.Interfaces;

public interface ITokenService
{
    /// <summary>
    /// Gera um token JWT contendo as informações de autenticação do usuário.
    /// </summary>
    /// <param name="email">E-mail do usuário.</param>
    /// <param name="role">Perfil (role) do usuário.</param>
    /// <returns>Token JWT gerado.</returns>
    string GenerateJwtToken(string email, string role);
}
