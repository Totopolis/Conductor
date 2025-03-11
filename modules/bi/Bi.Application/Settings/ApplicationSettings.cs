using Application.Shared.Settings;

namespace Bi.Application.Settings;

public sealed class ApplicationSettings : ISettings
{
    public static string SectionName => "Application";

    public required NotificationsSettings Notifications { get; init; }

    public sealed class NotificationsSettings
    {
        public bool Enable { get; init; }

        public required IReadOnlyList<string> Emails { get; init; }
    }
}
