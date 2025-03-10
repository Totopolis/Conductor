using Bi.Domain.DataSources;
using Domain.Shared;

namespace Bi.Domain.Events;

// TODO: dont mix domain event with sourced event
public sealed record NeedUpdateDbSource(
    DbSourceId Id,
    string Name,
    string PrivateNotes,
    string Description,
    string ConnectionString,
    DbSourceSchemaMode SchemaMode,
    string ManualSchema) : IDomainEvent, IBiPartitionedEvent
{
    public Guid PartitionKey => Id.Value;
}
