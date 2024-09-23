using Conductor.Domain.Deployments;
using NodaTime;

namespace Conductor.Domain.Processes;

public sealed class Instance : AggregateRoot<InstanceId>
{
    private Instance(
        InstanceId id,
        DeploymentId deploymentId,
        ProcessId processId,
        RevisionId revisionId,
        Instant created) : base(id)
    {
        DeploymentId = deploymentId;
        ProcessId = processId;
        RevisionId = revisionId;
        Created = created;
    }

    public DeploymentId DeploymentId { get; init; }

    public ProcessId ProcessId { get; init; }

    public RevisionId RevisionId { get; init; }

    public Instant Created { get; init; }

    public static Instance Create(
        DeploymentId deploymentId,
        ProcessId processId,
        RevisionId revisionId,
        Instant now)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(Guid.Empty, deploymentId.Id);
        ArgumentOutOfRangeException.ThrowIfEqual(Guid.Empty, processId.Id);
        ArgumentOutOfRangeException.ThrowIfEqual(Guid.Empty, revisionId.Id);

        var id = new InstanceId(Guid.NewGuid());

        var instance = new Instance(
            id,
            deploymentId,
            processId,
            revisionId,
            now);

        return instance;
    }
}
