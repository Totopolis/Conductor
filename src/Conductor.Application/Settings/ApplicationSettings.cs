namespace Conductor.Application.Settings;

public sealed class ApplicationSettings
{
    public const string SectionName = "Application";

    /// <summary>
    /// Unique number for all instances of all entities.
    /// </summary>
    public required bool GeneralNumbering { get; init; } = false;

    /// <summary>
    /// Check deploy status each n-seconds.
    /// </summary>
    public required int RefreshSecondsWhileDeploy { get; init; } = 2;
}
