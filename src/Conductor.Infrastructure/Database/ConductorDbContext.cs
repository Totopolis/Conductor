using Conductor.Domain.Deployments;
using Conductor.Domain.Processes;
using Conductor.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Conductor.Infrastructure.Database;

public class ConductorDbContext : DbContext
{
    public const string TimestampType = "timestamp with time zone";

    private readonly string _connectionString;
    private readonly ILoggerFactory _loggerFactory;

    public ConductorDbContext(
        IOptions<InfrastructureSettings> options,
        ILoggerFactory loggerFactory)
    {
        _connectionString = options.Value.DatabaseConnectionString;
        _loggerFactory = loggerFactory;
    }

    // ATTENTION: Only dev env use
    public async Task EnsureDatabaseStructureCreated(CancellationToken ct = default)
    {
        var sql = @"
-- Recreate the schema
DROP SCHEMA public CASCADE;
CREATE SCHEMA public;

-- Restore default permissions
-- GRANT ALL ON SCHEMA public TO postgres;
GRANT ALL ON SCHEMA public TO public;";

        await Database.ExecuteSqlRawAsync(sql, ct);
        await Database.EnsureCreatedAsync(ct);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseNpgsql(_connectionString, o => o.UseNodaTime())
            .UseLoggerFactory(_loggerFactory);
        // .EnableSensitiveDataLogging();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // configurationBuilder.ConfigureSmartEnum();
        base.ConfigureConventions(configurationBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");

        ConfigureProcessModel(modelBuilder);
        ConfigureDeploymentModel(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private static void ConfigureProcessModel(ModelBuilder modelBuilder)
    {
        var process = modelBuilder.Entity<Process>();
        process
            .ToTable("process")
            .HasKey(x => x.Id)
            .HasName("process_id");

        process
            .Property(x => x.Id)
            .ValueGeneratedNever()
            .HasColumnName("id")
            .HasConversion(id => id.Id, val => new ProcessId(val));

        process
            .Property(x => x.Name)
            .HasColumnName("name")
            .IsRequired();

        process
            .Property(x => x.DisplayName)
            .HasColumnName("display_name")
            .IsRequired();

        process
            .Property(x => x.Description)
            .HasColumnName("description");

        process
            .Property(x => x.Created)
            .HasColumnName("created")
            .HasColumnType(TimestampType)
            .IsRequired();

        process
            .Property(x => x.Number)
            .HasColumnName("number")
            .IsRequired();

        process
            .OwnsMany(x => x.Revisions, sb =>
            {
                sb.ToTable("revision");

                sb.WithOwner().HasForeignKey("ProcessId");

                sb.Property(x => x.Id)
                .ValueGeneratedNever()
                .HasColumnName("id")
                .HasConversion(id => id.Id, val => new RevisionId(val));

                sb.Property(x => x.Created)
                .HasColumnName("created")
                .HasColumnType(TimestampType)
                .IsRequired();

                sb.Property(x => x.Number).HasColumnName("number").IsRequired();

                sb.Property(x => x.IsDraft).HasColumnName("is_draft").IsRequired();

                sb.Property(x => x.ReleaseNotes).HasColumnName("release_notes");

                sb.Property(x => x.Content)
                .HasColumnName("content")
                .HasColumnType("jsonb")
                .IsRequired();
            });
    }

    private static void ConfigureDeploymentModel(ModelBuilder modelBuilder)
    {
        var deployment = modelBuilder.Entity<Deployment>();
        deployment
            .ToTable("deployment")
            .HasKey(x => x.Id)
            .HasName("deployment_id");

        deployment
            .Property(x => x.Id)
            .ValueGeneratedNever()
            .HasColumnName("id")
            .HasConversion(id => id.Id, val => new DeploymentId(val));

        deployment
            .Property(x => x.ProcessId)
            .HasColumnName("process_id")
            .HasConversion(id => id.Id, val => new ProcessId(val));

        deployment
            .Property(x => x.RevisionId)
            .HasColumnName("revision_id")
            .HasConversion(id => id.Id, val => new RevisionId(val));

        deployment
            .Property(x => x.Created)
            .HasColumnName("created")
            .HasColumnType(TimestampType)
            .IsRequired();

        deployment
            .Property(x => x.Number)
            .HasColumnName("number")
            .IsRequired();

        // TODO: map State smartenum

        deployment
            .Property(x => x.Notes)
            .HasColumnName("notes");

        // TODO: map targets
    }
}
