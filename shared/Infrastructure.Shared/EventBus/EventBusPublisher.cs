using Application.Shared.Abstractions;
using Domain.Shared;
using MassTransit;

namespace Infrastructure.Shared.EventBus;

internal class EventBusPublisher : IEventBusPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public EventBusPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task Publish<T>(T message, CancellationToken ct) where T : class
    {
        return _publishEndpoint.Publish(message, ct);
    }

    public Task PublishDomainEvent(IDomainEvent domainEvent, CancellationToken ct)
    {
        var domainEventType = domainEvent.GetType();

        return _publishEndpoint.Publish(
            message: domainEvent,
            messageType: domainEventType,
            callback: ctx =>
            {
            });
    }
}
