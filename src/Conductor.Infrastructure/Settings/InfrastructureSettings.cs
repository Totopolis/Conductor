namespace Conductor.Infrastructure.Settings;

public sealed class InfrastructureSettings
{
    public const string SectionName = "Infrastructure";

    public required string DatabaseConnectionString { get; init; }
}
