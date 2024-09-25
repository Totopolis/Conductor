using Conductor.Domain.Abstractions;
using Conductor.Infrastructure.Database;
using Conductor.Infrastructure.EventBus;
using Conductor.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Settings.Extensions;

namespace Conductor.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSettingsWithValidation<
            InfrastructureSettings,
            InfrastructureSettingsValidator>(InfrastructureSettings.SectionName);

        // It is scoped service
        services.AddDbContext<ConductorDbContext>();

        services
            .AddTransient<IProcessRepository, ProcessRepository>()
            .AddTransient<IDeploymentRepository, DeploymentRepository>()
            .AddTransient<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IEventBus, EventBus.EventBus>();

        return services;
    }
}
