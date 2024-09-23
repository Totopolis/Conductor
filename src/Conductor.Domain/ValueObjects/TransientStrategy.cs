using CSharpFunctionalExtensions;

namespace Conductor.Domain.ValueObjects;

public sealed class TransientStrategy :
    EnumValueObject<TransientStrategy>,
    IEquatable<TransientStrategy>
{
    private TransientStrategy(string id) : base(id)
    {
    }

    /// <summary>
    /// Waiting for the process to stop.
    /// </summary>
    public static TransientStrategy Await = new("await");

    /// <summary>
    /// Kill immediately.
    /// </summary>
    public static TransientStrategy Kill = new("kill");

    /// <summary>
    /// Leave the process running.
    /// </summary>
    public static TransientStrategy Skip = new("skip");

    public bool Equals(TransientStrategy? other)
    {
        return this == other;
    }
}
