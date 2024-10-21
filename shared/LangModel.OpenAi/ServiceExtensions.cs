using LangModel.Abstractions.Answerizer;
using LangModel.Abstractions.Vectorizer;
using LangModel.OpenAi.Answerizer;
using LangModel.OpenAi.Settings;
using LangModel.OpenAi.Vectorizer;
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

        services.AddScoped<IAnswerizerService, AnswerizerService>();
        services.AddTransient<IQuestionBuilder, OpenAiQuestionBuilder>();

        services.AddScoped<IVectorizerService, VectorizerService>();

        services.AddScoped<TracerComposite>();

        // TODO: use separated named oai services
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
