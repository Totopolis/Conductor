using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Server.Abstractions;
using Server.Boot.Consul;
using Server.Boot.MaintenanceBot;
using Server.Boot.SystemVersion;
using Settings.Extensions;
using Winton.Extensions.Configuration.Consul;

namespace Server.Boot;

public static partial class ServiceExtensions
{
    public static IServiceCollection AddSystemVersion(
        this IServiceCollection services)
    {
        services.AddSingleton<ISystemInfo, SystemInfo>();
        return services;
    }

    public static IServiceCollection AddMaintenanceBot(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IMaintenanceBot, MaintenanceBotSender>();

        services.AddSettingsWithValidation<
            MaintenanceBotSettings,
            MaintenanceBotSettingsValidator>(MaintenanceBotSettings.SectionName);

        services.AddHostedService<MaintenanceBotHostedService>();

        return services;
    }

    public static void MergeServerSettingsFromConsul(
        this IConfigurationManager configurationManager,
        IConfiguration configuration)
    {
        var settings = configuration.ValidateAndReturnPreBuildSettings<
            ConsulSettings,
            ConsulSettingsValidator>(ConsulSettings.SectionName);

        configurationManager
            .AddConsul(
                $"{settings.ApplicationName}/{settings.EnvironmentName}",
                opt =>
                {
                    opt.ConsulConfigurationOptions = cco =>
                    {
                        cco.Address = new Uri(settings.Url);
                        cco.Token = settings.Token;
                    };
                })
            .Build();
    }
}
