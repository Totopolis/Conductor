using Conductor.Application.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Settings.Extensions;

namespace Conductor.Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSettingsWithValidation<
            CommonSettings,
            CommonSettingsValidator>(CommonSettings.SectionName);

        return services;
    }
}
