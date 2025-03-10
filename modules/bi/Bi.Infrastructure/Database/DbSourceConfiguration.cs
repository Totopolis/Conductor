using Bi.Domain.DataSources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bi.Infrastructure.Database;

internal sealed class DbSourceConfiguration : IEntityTypeConfiguration<DbSource>
{
    public void Configure(EntityTypeBuilder<DbSource> builder)
    {
        builder
            .ToTable("dbsource")
            .HasKey(x => x.Id)
            .HasName("dbsource_id");

        builder
            .Property(x => x.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder
            .Property(x => x.Kind)
            .HasColumnName("kind")
            .HasConversion(x => x.Value, x => DbSourceKind.FromValue(x));

        builder
            .Property(x => x.Name)
            .HasColumnName("name")
            .IsRequired();

        builder
            .Property(x => x.PrivateNotes)
            .HasColumnName("private_notes")
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
            .Property(x => x.SchemaMode)
            .HasColumnName("schema_mode")
            .HasConversion(x => x.Value, x => DbSourceSchemaMode.FromValue(x));

        builder
            .Property(x => x.Schema)
            .HasColumnName("schema")
            .IsRequired();

        builder
            .Property(x => x.State)
            .HasColumnName("state")
            .HasConversion(x => x.Value, x => DbSourceState.FromValue(x));

        builder
            .Property(x => x.StateChanged)
            .HasColumnName("state_changed")
            .HasColumnType(BiDbContext.TimestampType)
            .IsRequired();
    }
}
