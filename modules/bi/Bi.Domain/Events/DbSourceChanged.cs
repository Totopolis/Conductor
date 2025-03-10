using Bi.Domain.DataSources;
using Domain.Shared;

namespace Bi.Domain.Events;

public sealed record DbSourceChanged(
    DbSourceId Id,
    string Name) : IDomainEvent, IBiPartitionedEvent
{
    public Guid PartitionKey => Id.Value;
};
