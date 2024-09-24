using Conductor.Domain.Deployments;
using NodaTime;
using System.Text.Json;

namespace Conductor.Domain.Processes;

public sealed class Instance : AggregateRoot<InstanceId>
{
    private Instance(
        InstanceId id,
        DeploymentId deploymentId,
        ProcessId processId,
        RevisionId revisionId,
        Instant created,
        JsonElement state) : base(id)
    {
        DeploymentId = deploymentId;
        ProcessId = processId;
        RevisionId = revisionId;
        Created = created;
        State = state;
    }

    public DeploymentId DeploymentId { get; init; }

    public ProcessId ProcessId { get; init; }

    public RevisionId RevisionId { get; init; }

    public Instant Created { get; init; }

    public JsonElement State { get; private set; }

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
            now,
            Helpers.EmptyJsonElement());

        return instance;
    }
}
