using Conductor.Domain.Processes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Conductor.Infrastructure.Database;

internal sealed class ProcessConfigurations : IEntityTypeConfiguration<Process>
{
    public void Configure(EntityTypeBuilder<Process> builder)
    {
        builder
            .ToTable("process")
            .HasKey(x => x.Id)
            .HasName("process_id");

        builder
            .Property(x => x.Id)
            .ValueGeneratedNever()
            .HasColumnName("id")
            .HasConversion(id => id.Id, val => new ProcessId(val));

        builder
            .Property(x => x.Name)
            .HasColumnName("name")
            .IsRequired();

        builder
            .Property(x => x.DisplayName)
            .HasColumnName("display_name")
            .IsRequired();

        builder
            .Property(x => x.Description)
            .HasColumnName("description");

        builder
            .Property(x => x.Created)
            .HasColumnName("created")
            .HasColumnType(ConductorDbContext.TimestampType)
            .IsRequired();

        builder
            .Property(x => x.Number)
            .HasColumnName("number")
            .IsRequired();

        builder
            .OwnsMany(x => x.Revisions, sb =>
            {
                sb.ToTable("revision")
                .HasKey(x => x.Id)
                .HasName("revision_id");

                sb.Property(x => x.Id)
                .ValueGeneratedNever()
                .HasColumnName("id")
                .HasConversion(id => id.Id, val => new RevisionId(val));

                sb.WithOwner().HasForeignKey(x => x.ProcessId);

                sb.Property(x => x.ProcessId)
                .HasColumnName("process_id")
                .HasConversion(x => x.Id, val => new ProcessId(val))
                .IsRequired();

                sb.Property(x => x.Created)
                .HasColumnName("created")
                .HasColumnType(ConductorDbContext.TimestampType)
                .IsRequired();

                sb.Property(x => x.Number)
                .HasColumnName("number")
                .IsRequired();

                sb.Property(x => x.IsDraft)
                .HasColumnName("is_draft")
                .IsRequired();

                sb.Property(x => x.ReleaseNotes)
                .HasColumnName("release_notes")
                .HasColumnType("text");

                sb.Property(x => x.Content)
                .HasColumnName("content")
                .HasColumnType("jsonb")
                .IsRequired();
            });
    }
}
