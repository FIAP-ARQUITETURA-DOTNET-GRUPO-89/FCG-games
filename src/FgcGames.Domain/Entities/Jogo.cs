using FgcGames.Domain.Enum;
using FgcGames.Domain.Interfaces;

namespace FgcGames.Domain.Entities;

public class Jogo : Entity, IAggregateRoot
{
    public DateTime DataCriacao { get; private set; }
    public DateTime DataLancamento { get; private set; }
    public string Nome { get; private set; }
    public string Descricao { get; private set; }
    public decimal Preco { get; private set; }
    public ClassificacaoEtaria ClassificacaoEtaria { get; private set; }
    public bool Inativo { get; private set; }

    public Jogo(DateTime dataLancamento, string nome, string descricao, decimal preco, ClassificacaoEtaria classificacaoEtaria) 
    {
        DataCriacao = DateTime.Now;
        DataLancamento = dataLancamento;
        Nome = nome;
        Descricao = descricao;
        Preco = preco;
        ClassificacaoEtaria = classificacaoEtaria;
    }

    public void AlterarPreco(decimal novoPreco)
    {
        // implementar
        Preco = novoPreco;
    }

    public bool UsuarioPodeJogar(Usuario usuario)
    {
        var idadeUsuario = usuario.CalcularIdade();
        var idadeMinima = (int)ClassificacaoEtaria;

        return idadeUsuario >= idadeMinima;
    }

    public void Inativar() => Inativo = true;

    public void Atualizar(string nome, string descricao, DateTime dataLancamento, ClassificacaoEtaria classificacaoEtaria)
    {
        Nome = nome;
        Descricao = descricao;
        DataLancamento = dataLancamento;
        ClassificacaoEtaria = classificacaoEtaria;
    }
}
