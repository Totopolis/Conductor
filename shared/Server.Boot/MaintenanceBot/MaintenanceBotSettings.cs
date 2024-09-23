namespace Server.Boot.MaintenanceBot;

public class MaintenanceBotSettings
{
    public const string SectionName = "MaintenanceBot";

    public bool Enable { get; init; }

    public required string ApiKey { get; init; }

    public required string ChatId { get; init; }
}
