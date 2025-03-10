using Ardalis.SmartEnum;

namespace Bi.Domain.DataSources;

public class DbSourceState : SmartEnum<DbSourceKind>
{
    public static readonly DbSourceState Disabled = new(nameof(Disabled), 10);

    public static readonly DbSourceState Setup = new(nameof(Setup), 20);

    public static readonly DbSourceState ConnectionFailed = new(nameof(ConnectionFailed), 30);

    public static readonly DbSourceState SchemaNotAvailable = new(nameof(SchemaNotAvailable), 40);

    public static readonly DbSourceState NotValid = new(nameof(NotValid), 50);

    public static readonly DbSourceState Ready = new(nameof(Ready), 60);

    private DbSourceState(string name, int value) : base(name, value)
    {
    }

    public bool IsDisabled => this == Disabled;

    public bool IsEnabled => !IsDisabled;

    public bool IsSetup => this == Setup;

    public bool IsConnectionFailed => this == ConnectionFailed;

    public bool IsSchemaNotAvailable => this == SchemaNotAvailable;

    public bool IsNotValid => this == NotValid;

    public bool IsReady => this == Ready;

    public bool IsBusy => !IsReady;
}
