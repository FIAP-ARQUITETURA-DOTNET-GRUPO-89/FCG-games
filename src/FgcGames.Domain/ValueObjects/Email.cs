namespace FgcGames.Domain.ValueObjects;

public record Email(string Endereco)
{
    protected Email() : this(string.Empty) { }

    public static Email Create(string endereco)
    {
        if (string.IsNullOrWhiteSpace(endereco) || !endereco.Contains("@"))
            throw new ArgumentException("E-mail inválido.");

        return new Email(endereco.ToLower().Trim());
    }
}
