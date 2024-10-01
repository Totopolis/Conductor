using Conductor.Domain.Abstractions;
using Conductor.Infrastructure.Database;
using Conductor.Infrastructure.EventBus;
using Conductor.Infrastructure.Numbers;
using Conductor.Infrastructure.Settings;
using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
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

        services.AddScoped<INumberService, NumberService>();

        return services;
    }

    public static IServiceCollection AddMasstransitLocal(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            // DANGER: Auto registrations DONT WORK with INTERNALs consumers!
            // https://github.com/MassTransit/MassTransit/issues/2253
            // busConfigurator.AddConsumers(typeof(IEventBus).Assembly);

            // Handmage autoregistration
            var consumers = GetAllConsumersFromAssemblyContainsType<IEventBus>();
            busConfigurator.AddConsumers(consumers);

            busConfigurator.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureJsonSerializerOptions(jsonOpt =>
                {
                    jsonOpt.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
                    return jsonOpt;
                });

                cfg.UseMessageRetry(r =>
                {
                    r.Interval(3, TimeSpan.FromSeconds(5));
                });

                cfg.UseTimeout(r =>
                {
                    r.Timeout = TimeSpan.FromMinutes(2);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    private static Type[] GetAllConsumersFromAssemblyContainsType<T>()
    {
        var types = typeof(T).Assembly
            .GetTypes()
            .Where(RegistrationMetadata.IsConsumerOrDefinition)
            .ToArray();

        return types;
    }
}
