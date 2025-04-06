using Application.Shared;
using Application.Shared.Settings;
using Betalgo.Ranul.OpenAI.Extensions;
using Lang.Application;
using Lang.Application.Abstractions;
using Lang.Application.Handlers;
using Lang.Infrastructure.Services;
using Lang.Infrastructure.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZiggyCreatures.Caching.Fusion;

namespace Lang.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddLangInfrastructureOptions(
        this IServiceCollection services)
    {
        services.AddSettingsWithValidation<
            InfrastructureSettings,
            InfrastructureSettingsValidator,
            LangModule>();

        return services;
    }

    public static IServiceCollection AddLangInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IModule, LangModule>();

        services
            .AddTransient<IVectorizer, Vectorizer>()
            .AddTransient<IAnswerizer, Answerizer>();

        var settings = configuration.ValidateAndReturnPreBuildSettings<
            InfrastructureSettings,
            InfrastructureSettingsValidator,
            LangModule>();

        services.AddOpenAIService(opt =>
        {
            opt.ApiKey = settings.GptApiKey;
            opt.BaseDomain = settings.GptBaseDomain;
        });

        services.AddFusionCache(Constants.VectorizerCacheName)
            .WithMemoryCache(new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = Constants.VectorizerCacheSize
            }))
            .WithDefaultEntryOptions(opts =>
            {
                // opts.SetFactoryTimeouts(TimeSpan.FromSeconds(2));
                opts.SetFailSafe(
                    isEnabled: true,
                    maxDuration: TimeSpan.FromMinutes(15));
                
                opts.Duration = TimeSpan.FromDays(1);
                opts.Size = 1;
            });

        return services;
    }
}
