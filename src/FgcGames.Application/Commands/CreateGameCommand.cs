using FgcGames.Domain.Enum;

namespace FgcGames.Application.Commands
{
    public record CreateGameCommand(
        string Nome,
        string Descricao,
        decimal Preco,
        DateTime DataLancamento,
        ClassificacaoEtaria ClassificacaoEtaria
    );
}
