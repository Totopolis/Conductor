using FluentValidation;

namespace Server.Boot.MaintenanceBot;

internal class MaintenanceBotSettingsValidator : AbstractValidator<MaintenanceBotSettings>
{
    public MaintenanceBotSettingsValidator()
    {
        RuleFor(x => x.ChatId)
            .NotEmpty()
            .When(x => x.Enable);

        RuleFor(x => x.ApiKey)
            .NotEmpty()
            .When(x => x.Enable);
    }
}
