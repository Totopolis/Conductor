using Conductor.Domain.Abstractions;
using Conductor.Domain.Deployments;
using Microsoft.EntityFrameworkCore;

namespace Conductor.Infrastructure.Database;

internal sealed class DeploymentRepository : IDeploymentRepository
{
    private readonly ConductorDbContext _db;

    public DeploymentRepository(ConductorDbContext db)
    {
        _db = db;
    }

    public async Task<Deployment> GetById(DeploymentId deploymentId, CancellationToken ct)
    {
        var finded = await _db.Set<Deployment>()
            .Include(x => x.Targets)
            .Where(x => x.Id == deploymentId)
            .FirstAsync(ct);

        return finded;
    }
}
