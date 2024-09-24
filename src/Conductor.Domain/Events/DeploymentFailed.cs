using Conductor.Domain.Deployments;
using Conductor.Domain.Primitives;

namespace Conductor.Domain.Events;

public sealed record DeploymentFailed(
    DeploymentId DeploymentId,
    string Reason) : IDomainEvent;
