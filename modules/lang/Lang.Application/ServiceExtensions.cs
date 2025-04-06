using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lang.Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddLangApplicationOptions(
        this IServiceCollection services)
    {
        /*services.AddSettingsWithValidation<
            ApplicationSettings,
            ApplicationSettingsValidator,
            LangModule>();*/

        return services;
    }

    public static IServiceCollection AddLangApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg
            .RegisterServicesFromAssemblyContaining<LangModule>());

        return services;
    }
}
