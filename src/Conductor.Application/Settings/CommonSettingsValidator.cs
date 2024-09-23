using FluentValidation;

namespace Conductor.Application.Settings;

internal sealed class CommonSettingsValidator : AbstractValidator<CommonSettings>
{
    public CommonSettingsValidator()
    {
        RuleFor(x => x.RefreshSecondsWhileDeploy)
            .GreaterThan(0);
    }
}
