namespace FgcGames.Domain.ValueObjects;

public record Senha(string Password)
{
    public static Senha Create(string senha) =>
        //regras pra senha
        new(senha);
}
