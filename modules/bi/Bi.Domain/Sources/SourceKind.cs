using Ardalis.SmartEnum;

namespace Bi.Domain.Sources;

public class SourceKind : SmartEnum<SourceKind>
{
    public static readonly SourceKind Postgres = new(nameof(Postgres), 10);
    public static readonly SourceKind SqlServer = new(nameof(SqlServer), 20);
    public static readonly SourceKind Clickhouse = new(nameof(Clickhouse), 30);
    public static readonly SourceKind MongoDb = new(nameof(MongoDb), 40);
    public static readonly SourceKind Redis = new(nameof(Redis), 50);

    private SourceKind(string name, int value) : base(name, value)
    {
    }

    public bool IsPostgres => this == Postgres;
}
