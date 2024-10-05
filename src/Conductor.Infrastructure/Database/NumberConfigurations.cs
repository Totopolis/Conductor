using Conductor.Domain.Numbers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Conductor.Infrastructure.Database;

internal sealed class NumberConfigurations : IEntityTypeConfiguration<Number>
{
    public void Configure(EntityTypeBuilder<Number> builder)
    {
        builder
            .ToTable("number")
            .HasKey(x => x.Id)
            .HasName("number_id");

        builder
            .Property(x => x.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder
            .Property(x => x.Kind)
            .HasColumnName("kind")
            .IsRequired()
            .HasConversion(state => state.Value, val => GeneratorKind.FromValue(val));

        builder
            .Property(x => x.Value)
            .HasColumnName("value")
            .IsRequired();

        builder.HasData(
            Number.SeedData(GeneratorKind.General),
            Number.SeedData(GeneratorKind.Process),
            Number.SeedData(GeneratorKind.Deployment));
    }
}
