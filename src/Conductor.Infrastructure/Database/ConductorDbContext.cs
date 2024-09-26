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

        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
