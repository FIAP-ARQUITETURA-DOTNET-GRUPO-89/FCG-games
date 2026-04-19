namespace FgcGames.Domain.ValueObjects;

public record Senha(string Password)
{
    protected Senha() : this(string.Empty) { }

    public static Senha Create(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha) || senha.Length < 8)
            throw new ArgumentException("A senha deve ter pelo menos 8 caracteres.");

        return new Senha(senha);
    }
}
