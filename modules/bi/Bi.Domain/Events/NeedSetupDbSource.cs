using Bi.Domain.DataSources;
using Domain.Shared;

namespace Bi.Domain.Events;

// TODO: dont mix domain event with sourced event
public sealed record NeedSetupDbSource(
    DbSourceId Id,
    string Name) : IDomainEvent, IBiPartitionedEvent
{
    public Guid PartitionKey => Id.Value;
}
