using FluentValidation;

namespace Conductor.Application.Settings;

internal sealed class ApplicationSettingsValidator : AbstractValidator<ApplicationSettings>
{
    public ApplicationSettingsValidator()
    {
        RuleFor(x => x.RefreshSecondsWhileDeploy)
            .GreaterThan(0);
    }
}
