using Conductor.Domain.Deployments;

namespace Conductor.Domain.Abstractions;

public interface IDeploymentRepository
{
    Task<Deployment> GetById(DeploymentId deploymentId, CancellationToken ct);
}
