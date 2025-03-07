using Bi.Domain.Abstractions;
using Bi.Infrastructure.Database;

namespace Bi.Infrastructure.Repositories;

internal class UnitOfWork : IUnitOfWork
{
    private readonly BiDbContext _dbContext;

    public UnitOfWork(BiDbContext context)
    {
        _dbContext = context;
    }

    public async Task SaveChanges(CancellationToken ct)
    {
        // var count =
        await _dbContext.SaveChangesAsync(ct);
    }
}
