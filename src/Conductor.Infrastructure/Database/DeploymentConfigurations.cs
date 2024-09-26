using Conductor.Domain.Deployments;
using Conductor.Domain.Processes;
using Conductor.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Conductor.Infrastructure.Database;

internal sealed class DeploymentConfigurations : IEntityTypeConfiguration<Deployment>
{
    public void Configure(EntityTypeBuilder<Deployment> builder)
    {
        builder
            .ToTable("deployment")
            .HasKey(x => x.Id)
            .HasName("deployment_id");

        builder
            .Property(x => x.Id)
            .ValueGeneratedNever()
            .HasColumnName("id")
            .HasConversion(id => id.Id, val => new DeploymentId(val));

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
            .Property(x => x.State)
            .HasColumnName("state")
            // TODO: fix ot
            .HasConversion(state => state.Id, val => DeploymentState.FromId(val).Value)
            .IsRequired();


        builder
            .Property(x => x.Notes)
            .HasColumnName("notes");

        builder
            .OwnsMany(x => x.Targets, sb =>
            {
                sb.ToTable("target")
                .HasKey(x => x.Id)
                .HasName("target_id");

                sb.Property(x => x.Id)
                .ValueGeneratedNever()
                .HasColumnName("id")
                .HasConversion(id => id.Id, val => new DeploymentTargetId(val));

                sb.WithOwner()
                .HasForeignKey(x => x.DeploymentId);

                sb.Property(x => x.DeploymentId)
                .HasColumnName("deployment_id")
                .HasConversion(id => id.Id, val => new DeploymentId(val))
                .IsRequired();

                sb.WithOwner()
                 .HasForeignKey(x => x.ProcessId);

                sb.Property(x => x.ProcessId)
                .HasColumnName("process_id")
                .HasConversion(id => id.Id, val => new ProcessId(val))
                .IsRequired();

                sb.WithOwner()
                .HasForeignKey(x => x.RevisionId);

                sb.Property(x => x.RevisionId)
                .HasColumnName("revision_id")
                .HasConversion(id => id.Id, val => new RevisionId(val))
                .IsRequired();

                sb.Property(x => x.ParallelCount)
                .HasColumnName("parallel_count")
                .IsRequired();

                sb.Property(x => x.BufferSize)
                .HasColumnName("buffer_size")
                .IsRequired();
            });
    }
}
