using FgcGames.Domain.Entities;
using FgcGames.Domain.Enum;
using FgcGames.Domain.ValueObjects;
using FgcGames.Infra.Database;
using FgcGames.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FgcGames.Infra.Seed;

/// <summary>
/// Responsável por popular o banco de dados com dados iniciais utilizados exclusivamente em ambiente de desenvolvimento.
/// </summary>
public static class DevDatabaseSeeder
{
    public static async Task SeedAsync(FgcGamesContext context, ISenhaHasherService senhaHasher)
    {
        var emails = new[]
        {
            "maria.silva@email.com",
            "joao.silva@email.com"
        };

        var usuariosExistentes = await context.Usuarios
            .Where(u => emails.Contains(u.Email.Endereco))
            .AnyAsync();

        if (usuariosExistentes)
        {
            return;
        }

        var usuarios = new List<Usuario>
        {
            new(
                nome: "Maria Silva",
                dataNascimento: new DateTime(1990, 1, 1),
                email: Email.Create("maria.silva@email.com"),
                senha: Senha.FromHash(senhaHasher.Hash("Abc!1234")),
                userRole: UserRole.Admin
            ),
            new(
                nome: "João Silva",
                dataNascimento: new DateTime(1995, 1, 1),
                email: Email.Create("joao.silva@email.com"),
                senha: Senha.FromHash(senhaHasher.Hash("Abc!1234")),
                userRole: UserRole.User
            )
        };

        await context.Usuarios.AddRangeAsync(usuarios);
        await context.SaveChangesAsync();
    }
}
