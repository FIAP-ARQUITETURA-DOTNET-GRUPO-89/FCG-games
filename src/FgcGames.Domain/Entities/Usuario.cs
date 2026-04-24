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

    protected Usuario() 
    {

    }

    public Usuario(string nome, DateTime dataNascimento, Email email, Senha senha, UserRole userRole)
    {
        DataCriacao = DateTime.UtcNow;
        Nome = nome;
        DataNascimento = dataNascimento;
        Email = email;
        Senha = senha;
        Role = userRole;
    }

    public void AtualizarPerfil(string nome, DateTime dataNascimento)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome não pode estar vazio.");

        if (dataNascimento > DateTime.Now)
            throw new ArgumentException("A data de nascimento não pode ser uma data futura.");

        Nome = nome;
        DataNascimento = dataNascimento;
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
