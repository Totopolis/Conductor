using Bi.Domain.Abstractions;
using Bi.Domain.DataSources;
using Bi.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Bi.Infrastructure.Repositories;

internal sealed class DbSourceRepository : IDbSourceRepository
{
    private readonly BiDbContext _dbContext;

    public DbSourceRepository(BiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(DbSource dataSource)
    {
        _dbContext.Set<DbSource>().Add(dataSource);
    }

    public async Task<DbSource?> Find(DbSourceId id, CancellationToken ct)
    {
        var finded = await _dbContext
            .Set<DbSource>()
            .FindAsync(id, ct);

        return finded;
    }

    public async Task<IReadOnlyList<DbSource>> GetAll(CancellationToken ct)
    {
        // DANGER: use shaping
        var all = await _dbContext
            .Set<DbSource>()
            .ToListAsync(ct);

        return all;
    }
}
