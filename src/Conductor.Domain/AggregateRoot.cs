using CSharpFunctionalExtensions;

namespace Conductor.Domain;

public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : IComparable<TId>
{
    protected AggregateRoot(TId id) : base(id)
    {
    }
}
