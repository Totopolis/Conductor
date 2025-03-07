using Application.Shared.Settings;

namespace Bi.Infrastructure.Settings;

public class InfrastructureSettings : ISettings
{
    public static string SectionName => "Infrastructure";

    public required string DatabaseConnectionString { get; init; }
}
