using FluentValidation;

namespace Bi.Infrastructure.Settings;

internal sealed class InfrastructureSettingsValidator : AbstractValidator<InfrastructureSettings>
{
    public InfrastructureSettingsValidator()
    {
        RuleFor(x => x.DatabaseConnectionString)
            .NotEmpty();
    }
}
