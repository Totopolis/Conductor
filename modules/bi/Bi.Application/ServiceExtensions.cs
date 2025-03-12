using Application.Shared.Settings;
using MassTransit.Middleware;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Bi.Application.Settings;
using Bi.Domain.Events;
using Bi.Application.Consumers;
using Domain.Shared;
using MassTransit.Context;

namespace Bi.Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddBiApplicationOptions(
        this IServiceCollection services)
    {
        services.AddSettingsWithValidation<
            ApplicationSettings,
            ApplicationSettingsValidator,
            BiModule>();

        return services;
    }

    public static IServiceCollection AddBiApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg
            .RegisterServicesFromAssemblyContaining<ApplicationSettings>());

        return services;
    }

    public static void AddBiApplicationConsumers(
        this IBusRegistrationConfigurator cfg)
    {
        var partitioner = new Partitioner(2, new Murmur3UnsafeHashGenerator());

        cfg.AddPartitionedConsumer<
            NeedSetupConsumer,
            NeedSetupSource>(partitioner);

        cfg.AddPartitionedConsumer<
            NeedUpdateConsumer,
            NeedUpdateSource>(partitioner);
    }

    private static void AddPartitionedConsumer<TCONSUMER, TMESSAGE>(
        this IBusRegistrationConfigurator cfg,
        Partitioner partitioner)
        where TCONSUMER : class, IConsumer<TMESSAGE>
        where TMESSAGE : class, IDomainEvent, IBiPartitionedEvent
    {
        cfg.AddConsumer<TCONSUMER>(y =>
        {
            y.UsePartitioner(partitioner, p =>
            {
                var scope = p as ConsumerConsumeContextScope<TCONSUMER, TMESSAGE>;
                if (scope is null)
                {
                    throw new ArgumentException("No context in event");
                }

                if (scope.Message is IBiPartitionedEvent partitionedEvent)
                {
                    return partitionedEvent.PartitionKey;
                }
                else
                {
                    throw new ArgumentException("Bi event without partitioning");
                }
            });
        });
    }
}
