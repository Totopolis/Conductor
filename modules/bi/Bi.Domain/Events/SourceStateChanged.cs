using Bi.Domain.Sources;
using Domain.Shared;

namespace Bi.Domain.Events;

// TODO: domain event goes to signalR
public sealed record SourceStateChanged(
    SourceId Id,
    string Name,
    SourceState State) : IDomainEvent, IBiPartitionedEvent
{
    public Guid PartitionKey => Id.Value;
}
