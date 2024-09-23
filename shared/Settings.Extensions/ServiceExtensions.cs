using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Settings.Extensions.Errors;

namespace Settings.Extensions;

public static class ServiceExtensions
{
    public static OptionsBuilder<TOptions> ValidateFluentValidation<TOptions>(
        this OptionsBuilder<TOptions> optionsBuilder)
    where TOptions : class
    {
        optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(
            provider => new FluentValidationOptions<TOptions>(optionsBuilder.Name, provider));

        return optionsBuilder;
    }

    public static OptionsBuilder<TOptions> AddSettingsWithValidation<TOptions, TValidator>(
        this IServiceCollection services,
        string configurationSection)
    where TOptions : class
    where TValidator : class, IValidator<TOptions>
    {
        // Add the validator
        services.AddScoped<IValidator<TOptions>, TValidator>();

        return services.AddOptions<TOptions>()
            .BindConfiguration(configurationSection)
            .ValidateFluentValidation()
            .ValidateOnStart();
    }

    public static TOptions ValidateAndReturnPreBuildSettings<TOptions, TValidator>(
        this IConfiguration configuration,
        string sectionName)
    where TOptions : class
    where TValidator : class, IValidator<TOptions>, new()
    {
        var section = configuration.GetRequiredSection(sectionName);
        var settings = section.Get<TOptions>();

        if (settings is null)
        {
            throw new MissedSectionException(sectionName);
        }

        var validator = new TValidator();
        validator.ValidateAndThrow(settings);

        return settings;
    }
}
