namespace FgcGames.Domain.ValueObjects;

public record Senha
{
    public string Hash { get; }

    private Senha(string hash) 
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new ArgumentException("O hash da senha não pode ser vazio.");
        }

        Hash = hash;
    }

    public static Senha FromHash(string hash) => new(hash);

    public static void ValidarTextoPuro(string senhaPura)
    {
        if (senhaPura.Length < 8 || senhaPura.Length > 12)
        {
            throw new ArgumentException("A senha deve ter entre 8 e 12 caracteres.");
        }
    }
}
