using FgcGames.Domain.Enum;

namespace FgcGames.Application.Commands
{
    public record UpdateGameCommand(
        Guid Id,
        string Nome,
        string Descricao,
        decimal Preco,
        DateTime DataLancamento,
        ClassificacaoEtaria ClassificacaoEtaria
    );
}
