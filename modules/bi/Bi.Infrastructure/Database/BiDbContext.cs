using Bi.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartEnum.EFCore;

namespace Bi.Infrastructure.Database;

public sealed class BiDbContext : DbContext
{
    public const string SchemaName = "bi";
    public const string TimestampType = "timestamp with time zone";

    private readonly ILoggerFactory _loggerFactory;
    private readonly string _connectionString;

    public BiDbContext()
    {
        _connectionString = default!;
        _loggerFactory = default!;
    }

    public BiDbContext(
        ILoggerFactory loggerFactory,
        IOptions<InfrastructureSettings> options)
    {
        _loggerFactory = loggerFactory;
        _connectionString = options.Value.DatabaseConnectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(
            connectionString: _connectionString,
            npgsqlOptionsAction: o => o
                .UseNodaTime()
                .MigrationsHistoryTable($"__{SchemaName}MigrationsHistory", SchemaName));

        optionsBuilder.UseLoggerFactory(_loggerFactory);

        optionsBuilder.EnableSensitiveDataLogging();

        base.OnConfiguring(optionsBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.ConfigureSmartEnum();
        configurationBuilder.RegisterAllInVogenEfCoreConverters();

        base.ConfigureConventions(configurationBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);

        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
