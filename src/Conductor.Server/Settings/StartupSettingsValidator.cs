using FluentValidation;

namespace Conductor.Server.Settings;

public sealed class StartupSettingsValidator : AbstractValidator<StartupSettings>
{
    public StartupSettingsValidator()
    {
    }
}
