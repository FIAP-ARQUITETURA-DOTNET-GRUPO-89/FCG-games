using FgcGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FgcGames.Infra.EntityTypeConfigurations;

public class TaskItemExampleEntityConfiguration : IEntityTypeConfiguration<TaskItemExample>
{
    public void Configure(EntityTypeBuilder<TaskItemExample> builder)
    {
        builder.ToTable("TaskItemExamples");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.IsCompleted)
               .IsRequired();
    }
}
