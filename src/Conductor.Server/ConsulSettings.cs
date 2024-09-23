namespace Conductor.Server;

public class ConsulSettings
{
    public const string SectionName = "Consul";

    public string ApplicationName { get; set; } = default!;

    public string EnvironmentName { get; set; } = default!;

    public string Uri { get; set; } = default!;

    public string Token { get; set; } = default!;
}
