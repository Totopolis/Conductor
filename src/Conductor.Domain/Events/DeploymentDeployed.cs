using Conductor.Domain.Deployments;
using Conductor.Domain.Primitives;

namespace Conductor.Domain.Events;

public sealed record DeploymentDeployed(DeploymentId DeploymentId) : IDomainEvent;
