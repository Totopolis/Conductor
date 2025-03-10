using Ardalis.SmartEnum;

namespace Bi.Domain.DataSources;

public class DbSourceSchemaMode : SmartEnum<DbSourceSchemaMode>
{
    public static readonly DbSourceSchemaMode Auto = new(nameof(Auto), 10);
    
    public static readonly DbSourceSchemaMode Manual = new(nameof(Manual), 20);

    public static readonly DbSourceSchemaMode NotUse = new(nameof(NotUse), 30);

    private DbSourceSchemaMode(string name, int value) : base(name, value)
    {
    }

    public bool IsAuto => this == Auto;

    public bool IsManual => this == Manual;

    public bool IsNotUse => this == NotUse;
}
