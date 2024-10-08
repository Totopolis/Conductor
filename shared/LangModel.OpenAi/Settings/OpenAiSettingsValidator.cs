using FluentValidation;

namespace LangModel.OpenAi.Settings;

internal sealed class OpenAiSettingsValidator : AbstractValidator<OpenAiSettings>
{
    public OpenAiSettingsValidator()
    {
        RuleFor(x => x.ApiKey)
                .NotEmpty();

        RuleFor(x => x.BaseDomain)
            .Must(x => Uri.TryCreate(x, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrWhiteSpace(x.BaseDomain))
            .WithMessage($"{nameof(OpenAiSettings.BaseDomain)} must be a valid URL");
    }
}
