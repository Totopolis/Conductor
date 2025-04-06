using FluentValidation;

namespace Lang.Infrastructure.Settings;

internal sealed class InfrastructureSettingsValidator : AbstractValidator<InfrastructureSettings>
{
    public InfrastructureSettingsValidator()
    {
        RuleFor(x => x.GptBaseDomain)
            .NotEmpty();

        RuleFor(x => x.GptApiKey)
            .NotEmpty();
    }
}
