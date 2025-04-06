using Application.Shared.Settings;

namespace Lang.Infrastructure.Settings;

public class InfrastructureSettings : ISettings
{
    public static string SectionName => "Infrastructure";

    public required string GptBaseDomain { get; init; }

    public required string GptApiKey { get; init; }
}
