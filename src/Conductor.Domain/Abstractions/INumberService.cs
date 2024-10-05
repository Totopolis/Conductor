using Conductor.Domain.Primitives;

namespace Conductor.Domain.Abstractions;

public interface INumberService
{
    /// <summary>
    /// Generate next number.
    /// All entities has one sequence.
    /// </summary>
    Task<int> GenerateGeneral<T, TID>(CancellationToken ct)
        where T : AggregateRoot<TID>
        where TID : struct, IComparable<TID>;

    /// <summary>
    /// Generate next number.
    /// Each entity has itself sequence.
    /// </summary>
    Task<int> GenerateSeparated<T, TID>(CancellationToken ct)
        where T : AggregateRoot<TID>
        where TID: struct, IComparable<TID>;
}
