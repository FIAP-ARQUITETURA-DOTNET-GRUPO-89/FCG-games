using FgcGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FgcGames.Infra.EntityTypeConfigurations;

public class JogoConfiguration : IEntityTypeConfiguration<Jogo>
{
    public void Configure(EntityTypeBuilder<Jogo> builder)
    {
        builder.ToTable("Jogos");

        builder.HasKey(j => j.Id);

        builder.Property(j => j.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(j => j.Descricao)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(j => j.Preco)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(j => j.ClassificacaoEtaria)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(j => j.DataCriacao).IsRequired().HasColumnType("timestamp without time zone");
        builder.Property(j => j.DataLancamento).IsRequired().HasColumnType("timestamp without time zone");
        builder.Property(j => j.Inativo).IsRequired();
    }
}
