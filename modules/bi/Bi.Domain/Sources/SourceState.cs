using Ardalis.SmartEnum;

namespace Bi.Domain.Sources;

public class SourceState : SmartEnum<SourceState>
{
    public static readonly SourceState Inactive = new(nameof(Inactive), 10);
    public static readonly SourceState Lock = new(nameof(Lock), 20);
    public static readonly SourceState Ready = new(nameof(Ready), 30);

    private SourceState(string name, int value) : base(name, value)
    {
    }

    public bool IsInactive => this == Inactive;

    public bool IsLock => this == Lock;

    public bool IsReady => this == Ready;
}
