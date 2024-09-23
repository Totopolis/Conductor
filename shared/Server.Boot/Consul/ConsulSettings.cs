namespace Server.Boot.Consul;

public record ConsulSettings
{
    public const string SectionName = "Consul";

    public required string Url { get; init; }

    public required string Token { get; init; }

    public required string ApplicationName { get; init; }

    public required string EnvironmentName { get; init; }
}
