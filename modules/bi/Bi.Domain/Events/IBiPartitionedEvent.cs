namespace Bi.Domain.Events;

public interface IBiPartitionedEvent
{
    Guid PartitionKey { get; }
}
