namespace Conductor.Domain.Primitives;

public interface IDomainEventsAccessor
{
    IReadOnlyCollection<IDomainEvent> GetDomainEvents();

    void ClearDomainEvents();

}
