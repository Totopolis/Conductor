using Ardalis.SmartEnum;

namespace Bi.Domain.Sources;

public class SourceState : SmartEnum<SourceState>
{
    public static readonly SourceState Disabled = new(nameof(Disabled), 10);
    public static readonly SourceState Setup = new(nameof(Setup), 20);
    public static readonly SourceState Failed = new(nameof(Failed), 30);
    public static readonly SourceState Ready = new(nameof(Ready), 1000);

    private SourceState(string name, int value) : base(name, value)
    {
    }

    public bool IsDisabled => this == Disabled;

    public bool IsSetup => this == Setup;

    public bool IsFailed => this == Failed;

    public bool IsReady => this == Ready;
}
