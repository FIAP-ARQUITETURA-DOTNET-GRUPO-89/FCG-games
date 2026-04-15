using FgcGames.Domain.Enum;
using FgcGames.Domain.Interfaces;
using FgcGames.Domain.ValueObjects;

namespace FgcGames.Domain.Entities;

public class Usuario : Entity, IAggregateRoot
{
    public DateTime DataCriacao { get; private set; }
    public string Nome { get; private set; }
    public DateTime DataNascimento { get; private set; }
    public Email Email { get; private set; }
    public Senha Senha { get; private set; }
    public UserRole Role { get; private set; }
    public bool Inativo { get; private set; }

    public Usuario(string nome, DateTime dataNascimento, Email email, Senha senha, UserRole userRole)
    {
        DataCriacao = DateTime.UtcNow;
        Nome = nome;
        DataNascimento = dataNascimento;
        Email = email;
        Senha = senha;
        Role = userRole;
    }

    public void AlterarSenha(Senha novaSenha)
    {
        //implementar
        Senha = novaSenha;
    }

    public void Inativar() => Inativo = true;

    public bool EhAdmin() => Role == UserRole.Admin;

    public int CalcularIdade()
    {
        //implementar
        return 1;
    }
}
