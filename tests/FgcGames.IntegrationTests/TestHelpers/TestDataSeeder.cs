using FgcGames.Application.Interfaces;
using FgcGames.Domain.Entities;
using FgcGames.Domain.Enum;
using FgcGames.Domain.ValueObjects;
using FgcGames.Infra.Database;
using Microsoft.EntityFrameworkCore;

namespace FgcGames.IntegrationTests.TestHelpers;

/// <summary>
/// Responsável por popular o banco de dados com dados iniciais necessários para os testes de integração.
/// </summary>
public static class TestDataSeeder
{
    public static async Task SeedAsync(FgcGamesContext context, ISenhaHasherService senhaHasher)
    {
        if (await context.Usuarios.AnyAsync())
        {
            return;
        }

        var admin = new Usuario(
            nome: "Admin",
            dataNascimento: new DateOnly(1990, 1, 1),
            email: new Email("admin@fgcgames.com"),
            senha: Senha.FromHash(senhaHasher.Hash("Abc!1234")),
            userRole: UserRole.Admin
        );

        var user = new Usuario(
            nome: "User",
            dataNascimento: new DateOnly(1995, 1, 1),
            email: new Email("user@fgcgames.com"),
            senha: Senha.FromHash(senhaHasher.Hash("Abc!1234")),
            userRole: UserRole.User
        );

        context.Usuarios.AddRange(admin, user);
        await context.SaveChangesAsync();
    }
}
