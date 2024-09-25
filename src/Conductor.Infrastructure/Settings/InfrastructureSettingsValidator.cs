using FluentValidation;

namespace Conductor.Infrastructure.Settings;

internal sealed class InfrastructureSettingsValidator : AbstractValidator<InfrastructureSettings>
{
    public InfrastructureSettingsValidator()
    {
        // TODO: check valid conn str
        RuleFor(x=>x.DatabaseConnectionString)
            .NotEmpty();
    }
}
