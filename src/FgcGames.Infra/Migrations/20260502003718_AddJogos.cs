using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FgcGames.Infra.Migrations;

/// <inheritdoc />
public partial class AddJogos : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Jogos",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                DataLancamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                Preco = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                ClassificacaoEtaria = table.Column<string>(type: "text", nullable: false),
                Inativo = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Jogos", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Jogos");
    }
}
