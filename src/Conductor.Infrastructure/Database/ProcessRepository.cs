using Conductor.Domain.Abstractions;
using Conductor.Domain.Processes;
using Microsoft.EntityFrameworkCore;

namespace Conductor.Infrastructure.Database;

public sealed class ProcessRepository : IProcessRepository
{
    private readonly ConductorDbContext _db;

    public ProcessRepository(ConductorDbContext db)
    {
        _db = db;
    }

    public async Task<Process> GetByIdWithAllRevisions(
        ProcessId processId,
        CancellationToken ct)
    {
        var finded = await _db.Set<Process>()
            .Include(x => x.Revisions)
            .Where(x => x.Id == processId)
            .FirstAsync(ct);

        return finded;
    }

    public async Task<Process> GetWithOneRevision(
        ProcessId processId,
        RevisionId revisionId,
        CancellationToken ct)
    {
        var finded = await _db.Set<Process>()
            .Include(x => x.Revisions)
            // TODO: filter revisionId
            .Where(x => x.Id == processId)
            .FirstAsync(ct);

        return finded;
    }
}
