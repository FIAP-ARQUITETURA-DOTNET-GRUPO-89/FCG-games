namespace FgcGames.Domain.ValueObjects;

public record Email(string Endereco)
{
    public static Email Create(string endereco) =>
        // colocar aqui regra
        new(endereco);
}
