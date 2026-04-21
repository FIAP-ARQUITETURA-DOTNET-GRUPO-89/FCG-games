using FgcGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FgcGames.Infra.EntityTypeConfigurations;

public class UsuarioEntityConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DataCriacao)
               .IsRequired()
               .HasColumnType("timestamp with time zone") 
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(x => x.Nome)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(x => x.DataNascimento)
               .IsRequired()
               .HasColumnType("date");

        builder.OwnsOne(x => x.Email, email =>
        {            
            email.Property(e => e.Endereco)
                 .HasColumnName("Email")
                 .HasMaxLength(254)
                 .IsRequired();

            email.HasIndex(e => e.Endereco).IsUnique();
        });

        builder.OwnsOne(x => x.Senha, senha =>
        {
            senha.Property(s => s.Hash)
                 .HasColumnName("Senha")
                 .HasMaxLength(60)
                 .IsRequired();
        });
               
        builder.Property(x => x.Role)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(20);

        builder.Property(x => x.Inativo)
               .IsRequired()
               .HasDefaultValue(false);
    }
}
