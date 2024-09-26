using Conductor.Domain.Processes;
using Conductor.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Conductor.Domain.Deployments;

/// <summary>
/// Target revision of the process.
/// </summary>
public sealed class DeploymentTarget : Entity<DeploymentTargetId>
{
    private DeploymentTarget(
        DeploymentTargetId id,
        DeploymentId deploymentId,
        ProcessId processId,
        RevisionId revisionId,
        int parallelCount,
        int bufferSize) : base(id)
    {
        ProcessId = processId;
        RevisionId = revisionId;
        ParallelCount = parallelCount;
        BufferSize = bufferSize;
    }

    public DeploymentId DeploymentId { get; init; }

    public ProcessId ProcessId { get; init; }

    public RevisionId RevisionId { get; init; }

    public int ParallelCount { get; init; }

    public int BufferSize { get; init; }

    /// <summary>
    /// Internal cause DeploymentTarget is part of the Deployment aggregate.
    /// </summary>
    internal static DeploymentTarget Create(
        DeploymentId deploymentId,
        Target target)
    {
        var id = new DeploymentTargetId(Guid.NewGuid());
        return new DeploymentTarget(
            id,
            deploymentId,
            target.ProcessId,
            target.RevisionId,
            target.ParallelCount,
            target.BufferSize);
    }
}
