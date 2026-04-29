using FgcGames.Domain.Enum;

namespace FgcGames.Application.Responses;

public record GameResponse(
    Guid Id,
    string Nome,
    string Descricao,
    decimal Preco,
    DateTime DataLancamento,
    ClassificacaoEtaria ClassificacaoEtaria
);
