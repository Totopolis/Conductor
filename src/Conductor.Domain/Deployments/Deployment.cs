using Conductor.Domain.Processes;
using Conductor.Domain.ValueObjects;
using NodaTime;

namespace Conductor.Domain.Deployments;

public sealed class Deployment : AggregateRoot<DeploymentId>
{
    private readonly List<DeploymentTarget> _targets = new();

    private Deployment(
        DeploymentId id,
        ProcessId processId,
        RevisionId revisionId,
        Instant created,
        int number,
        DeploymentState state,
        string notes) : base(id)
    {
        ProcessId = processId;
        RevisionId = revisionId;
        Created = created;
        Number = number;
        State = state;
        Notes = notes;
    }

    public ProcessId ProcessId { get; init; }

    public RevisionId RevisionId { get; init; }

    public Instant Created { get; init; }

    public int Number { get; private set; }

    public DeploymentState State { get; private set; }

    public string Notes { get; private set; }

    public IReadOnlyList<DeploymentTarget> Targets => _targets.AsReadOnly();

    public static Deployment CreateDraft(
        ProcessId processId,
        RevisionId revisionId,
        Instant now,
        int deploymentNumber,
        string notes)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(processId.Id, Guid.Empty);
        ArgumentOutOfRangeException.ThrowIfEqual(revisionId.Id, Guid.Empty);

        ArgumentOutOfRangeException.ThrowIfEqual(now, Instant.MinValue);
        ArgumentOutOfRangeException.ThrowIfEqual(now, Instant.MaxValue);

        ArgumentOutOfRangeException.ThrowIfEqual(deploymentNumber, int.MaxValue);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(deploymentNumber);

        var id = new DeploymentId(Guid.NewGuid());
        return new Deployment(
            id,
            processId,
            revisionId,
            created: now,
            deploymentNumber,
            state: DeploymentState.Draft,
            notes);
    }

    public void ChangeNotes(string notes)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(State, DeploymentState.Draft);

        Notes = notes;
    }

    public void ReplaceTargets(IReadOnlyList<Target> targets)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(State, DeploymentState.Draft);

        if (targets.DistinctBy(x => x.ProcessId).Count() != targets.Count())
        {
            throw new ArgumentException("Targets duplicates detected");
        }

        _targets.Clear();

        foreach (var target in targets)
        {
            _targets.Add(DeploymentTarget.Create(Id, target));
        }
    }

    public void SetTransient()
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(State, DeploymentState.Draft);
        State = DeploymentState.Transient;
    }

    public void SetFailed()
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(State, DeploymentState.Transient);
        State = DeploymentState.Failed;
    }

    public void SetDeployed()
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(State, DeploymentState.Transient);
        State = DeploymentState.Deployed;
    }

    public void SetDecommissioned()
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(State, DeploymentState.Deployed);
        State = DeploymentState.Decommissioned;
    }
}
