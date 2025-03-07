using Bi.Domain.Diagnostics;
using Domain.Shared;
using ErrorOr;

namespace Bi.Domain.DataSources;

public sealed class DataSource : AggregateRoot<DataSourceId>
{
    private DataSource(
        DataSourceId id,
        string description) : base(id)
    {
        Description = description;
    }

    public required string Name { get; init; }

    public string Description { get; private set; }

    public static ErrorOr<DataSource> CreateNew(
        string name,
        string description)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
        {
            return DomainErrors.BadNameFormat;
        }

        var id = DataSourceId.From(Guid.CreateVersion7());
        var dataSource = new DataSource(id, description)
        {
            Name = name
        };

        return dataSource;
    }

    public void ChangeDescription(string newDescription)
    {
        Description = newDescription;
    }
}
