using Bi.Domain.DataSources;

namespace Bi.Domain.Abstractions;

public interface IDataSourceRepository
{
    void Add(DataSource dataSource);

    Task<DataSource?> Find(DataSourceId id, CancellationToken ct);
}
