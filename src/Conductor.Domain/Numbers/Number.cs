using Conductor.Domain.Primitives;
using CSharpFunctionalExtensions;

namespace Conductor.Domain.Numbers;

// It does not contain the factory method.
// All available values are filled at database migrations.
public sealed class Number : Entity<Guid>
{
    private Number(Guid id) : base(id)
    {
    }

    public required GeneratorKind Kind { get; init; }

    public int Value { get; private set; }

    public static Number SeedData(GeneratorKind kind)
    {
        return new Number(Guid.NewGuid())
        {
            Kind = kind,
            Value = 1
        };
    }

    public void SetupNewValue(int newValue)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(newValue, 0);
        ArgumentOutOfRangeException.ThrowIfEqual(newValue, int.MaxValue);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(newValue, Value);

        Value = newValue;
    }
}
