using FluentValidation;

namespace Bi.Application.Settings;

public sealed class ApplicationSettingsValidator : AbstractValidator<ApplicationSettings>
{
    public ApplicationSettingsValidator()
    {
    }
}
