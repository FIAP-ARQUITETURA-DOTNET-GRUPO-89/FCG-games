using FgcGames.Domain.Entities;
using FgcGames.Domain.Enum;

namespace FgcGames.Tests.Domain;

public class GameTests
{
    [Fact]
    public void AlterarPreco_DeveAtualizarPreco()
    {
        var jogo = new Jogo(DateTime.UtcNow, "Jogo", "Desc", 50m, ClassificacaoEtaria.Livre);
        jogo.AlterarPreco(29.99m);
        Assert.Equal(29.99m, jogo.Preco);
    }

    [Fact]
    public void Inativar_DeveMarcarJogoComoInativo()
    {
        var jogo = new Jogo(DateTime.UtcNow, "Jogo", "Desc", 50m, ClassificacaoEtaria.Livre);
        jogo.Inativar();
        Assert.True(jogo.Inativo);
    }
}