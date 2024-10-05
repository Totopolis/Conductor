using Conductor.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Conductor.Infrastructure.Database;

// Generate migration sql script
// 0. Run VS Package Manager Console
// 1. dotnet tool install --global dotnet-ef
// 2. cd ./src/Conductor.Infrastructure
// 3. dotnet ef migrations add InitialCreate
// 4. dotnet ef migrations script -o ./Migrations/InitialCreate.sql
public class ConductorDbContext : DbContext
{
    public const string TimestampType = "timestamp with time zone";

    private readonly string _connectionString;
    private readonly ILoggerFactory _loggerFactory;

    public ConductorDbContext()
    {
        _connectionString = default!;
        _loggerFactory = default!;
    }

    public ConductorDbContext(
        IOptions<InfrastructureSettings> options,
        ILoggerFactory loggerFactory)
    {
        _connectionString = options.Value.DatabaseConnectionString;
        _loggerFactory = loggerFactory;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(
            connectionString: _connectionString,
            npgsqlOptionsAction: o => o.UseNodaTime());

        optionsBuilder.UseLoggerFactory(_loggerFactory);

        // builder.EnableSensitiveDataLogging();

        base.OnConfiguring(optionsBuilder);
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
