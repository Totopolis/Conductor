using Bi.Domain.Sources;
using Domain.Shared;

namespace Bi.Domain.Events;

// TODO: dont mix domain event with sourced event
public sealed record UpdateSource(
    SourceId Id,
    string Name,
    string UserNotes,
    string Description,
    string ConnectionString,
    string Schema) : IDomainEvent, IBiPartitionedEvent
{
    public Guid PartitionKey => Id.Value;
}
