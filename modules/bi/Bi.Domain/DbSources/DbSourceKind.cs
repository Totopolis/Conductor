using Ardalis.SmartEnum;

namespace Bi.Domain.DataSources;

public class DbSourceKind : SmartEnum<DbSourceKind>
{
    public static readonly DbSourceKind Postgres = new(nameof(Postgres), 1);
    public static readonly DbSourceKind Clickhouse = new(nameof(Clickhouse), 2);

    private DbSourceKind(string name, int value) : base(name, value)
    {
    }

    public bool IsPostgres => this == Postgres;

    public bool IsClickhouse => this == Clickhouse;
}
