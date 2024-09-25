using Conductor.Domain.Primitives;

namespace Conductor.Infrastructure.EventBus;

public interface IEventBus
{
    Task PublishDomainEventAsync(IDomainEvent domainEvent, CancellationToken ct = default);
}
