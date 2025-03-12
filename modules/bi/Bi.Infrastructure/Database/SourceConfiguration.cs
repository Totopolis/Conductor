using Bi.Domain.Sources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bi.Infrastructure.Database;

internal sealed class SourceConfiguration : IEntityTypeConfiguration<Source>
{
    public void Configure(EntityTypeBuilder<Source> builder)
    {
        builder
            .ToTable("source")
            .HasKey(x => x.Id)
            .HasName("source_id");

        builder
            .Property(x => x.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder
            .Property(x => x.Kind)
            .HasColumnName("kind")
            .HasConversion(x => x.Value, x => SourceKind.FromValue(x));

        builder
            .Property(x => x.Name)
            .HasColumnName("name")
            .IsRequired();

        builder
            .Property(x => x.UserNotes)
            .HasColumnName("user_notes")
            .IsRequired();

        builder
            .Property(x => x.Description)
            .HasColumnName("description")
            .IsRequired();

        builder
            .Property(x => x.ConnectionString)
            .HasColumnName("connection_string")
            .IsRequired();

        builder
            .Property(x => x.Schema)
            .HasColumnName("schema")
            .IsRequired();

        builder
            .Property(x => x.AiNotes)
            .HasColumnName("ai_notes")
            .IsRequired();

        builder
            .Property(x => x.State)
            .HasColumnName("state")
            .HasConversion(x => x.Value, x => SourceState.FromValue(x));

        builder
            .Property(x => x.StateChanged)
            .HasColumnName("state_changed")
            .HasColumnType(BiDbContext.TimestampType)
            .IsRequired();
    }
}
