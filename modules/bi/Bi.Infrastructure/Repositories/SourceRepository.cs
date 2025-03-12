using Bi.Domain.Abstractions;
using Bi.Domain.Sources;
using Bi.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Bi.Infrastructure.Repositories;

internal sealed class SourceRepository : ISourceRepository
{
    private readonly BiDbContext _dbContext;

    public SourceRepository(BiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(Source dataSource)
    {
        _dbContext.Set<Source>().Add(dataSource);
    }

    public async Task<Source?> Find(SourceId id, CancellationToken ct)
    {
        var finded = await _dbContext
            .Set<Source>()
            .FindAsync(id, ct);

        return finded;
    }

    public async Task<IReadOnlyList<Source>> GetAll(CancellationToken ct)
    {
        // DANGER: use shaping
        var all = await _dbContext
            .Set<Source>()
            .ToListAsync(ct);

        return all;
    }
}
