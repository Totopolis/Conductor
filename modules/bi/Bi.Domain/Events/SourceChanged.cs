using Bi.Domain.Sources;
using Domain.Shared;

namespace Bi.Domain.Events;

public sealed record SourceChanged(
    SourceId Id,
    string Name) : IDomainEvent, IBiPartitionedEvent
{
    public Guid PartitionKey => Id.Value;
};
