using LangModel.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Extensions;
using Settings.Extensions;

namespace LangModel.OpenAi;

public static class ServiceExtensions
{
    public static IServiceCollection AddLangModelOpenAiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSettingsWithValidation<
            OpenAiSettings,
            OpenAiSettingsValidator>(OpenAiSettings.SectionName);

        services.AddSingleton<ILangModel, OpenAiService>();
        services.AddSingleton<IQuestionBuilder, OpenAiQuestionBuilder>();

        services.AddOpenAIService(settings =>
        {
            // TODO: use options
            var langModelConfig = configuration
                .GetSection(OpenAiSettings.SectionName)
                .Get<OpenAiSettings>()!;

            settings.ApiKey = langModelConfig.ApiKey;
            settings.BaseDomain = langModelConfig.BaseDomain;
        });

        return services;
    }
}
