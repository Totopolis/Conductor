using Bi.Domain.Sources;
using Domain.Shared;

namespace Bi.Domain.Events;

// TODO: dont mix domain event with sourced event
public sealed record NeedSetupSource(
    SourceId Id,
    string Name) : IDomainEvent, IBiPartitionedEvent
{
    public Guid PartitionKey => Id.Value;
}
