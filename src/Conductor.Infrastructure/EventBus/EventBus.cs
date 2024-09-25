using Conductor.Domain.Primitives;
using MassTransit;

namespace Conductor.Infrastructure.EventBus;

internal sealed class EventBus : IEventBus
{
    private readonly IPublishEndpoint _publishEndpoint;

    public EventBus(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task PublishDomainEventAsync(IDomainEvent domainEvent, CancellationToken ct = default)
    {
        var domainEventType = domainEvent.GetType();

        return _publishEndpoint.Publish(
            message: domainEvent,
            messageType: domainEventType);
    }
}
