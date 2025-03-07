using Bi.Domain.Abstractions;
using Bi.Domain.DataSources;
using Bi.Infrastructure.Database;

namespace Bi.Infrastructure.Repositories;

internal sealed class DataSourceRepository : IDataSourceRepository
{
    private readonly BiDbContext _dbContext;

    public DataSourceRepository(BiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(DataSource dataSource)
    {
        _dbContext.Set<DataSource>().Add(dataSource);
    }

    public async Task<DataSource?> Find(DataSourceId id, CancellationToken ct)
    {
        var finded = await _dbContext
            .Set<DataSource>()
            .FindAsync(id, ct);

        return finded;
    }
}
