namespace FgcGames.Domain.ValueObjects;

public record Senha
{
    public string Hash { get; }

    //protected Senha() : this(string.Empty) { }

    private Senha(string hash) 
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new ArgumentException("O hash da senha não pode ser vazio.");
        }

        Hash = hash;
    }

    public static Senha FromHash(string hash) => new(hash);
}
