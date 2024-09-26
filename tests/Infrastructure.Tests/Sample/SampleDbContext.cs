using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tests.Sample;

internal class SampleDbContext : DbContext
{
    private readonly string _connectionString;

    public SampleDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task EnsureDatabaseStructureCreated(CancellationToken ct = default)
    {
        var sql = @"
-- Recreate the schema
DROP SCHEMA public CASCADE;
CREATE SCHEMA public;

-- Restore default permissions
GRANT ALL ON SCHEMA public TO postgres;
GRANT ALL ON SCHEMA public TO public;";

        await Database.ExecuteSqlRawAsync(sql, ct);
        // await Database.MigrateAsync(ct);

        await Database.EnsureCreatedAsync(ct);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseNpgsql(_connectionString)
            .EnableSensitiveDataLogging();

        // TODO: use utc - need avoid
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // configurationBuilder.ConfigureSmartEnum();

        base.ConfigureConventions(configurationBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");

        var foo = modelBuilder.Entity<FooEntity>();

        foo
            .ToTable("foo")
            .HasKey(x => x.Id)
            .HasName("foo_id");

        foo
            .Property(x => x.Id)
            .HasColumnName("id");

        foo
            .Property(x => x.One)
            .HasColumnName("one");

        foo
            .Property(x => x.Two)
            .HasColumnName("two");

        // foo.Ignore(x => x.Boo);
        foo.ComplexProperty(x => x.Boo);

        base.OnModelCreating(modelBuilder);
    }
}
