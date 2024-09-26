using Conductor.Domain.Events;
using Conductor.Domain.Primitives;
using Conductor.Domain.ValueObjects;
using NodaTime;

namespace Conductor.Domain.Deployments;

public sealed class Deployment : AggregateRoot<DeploymentId>
{
    private readonly List<DeploymentTarget> _targets = new();

    private Deployment(
        DeploymentId id,
        Instant created,
        int number,
        DeploymentState state,
        string notes) : base(id)
    {
        Created = created;
        Number = number;
        State = state;
        Notes = notes;
    }

    public Instant Created { get; init; }

    public int Number { get; private set; }

    public DeploymentState State { get; private set; }

    public string Notes { get; private set; }

    public IReadOnlyList<DeploymentTarget> Targets => _targets.AsReadOnly();

    public static Deployment CreateDraft(
        Instant now,
        int deploymentNumber,
        string notes)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(now, Instant.MinValue);
        ArgumentOutOfRangeException.ThrowIfEqual(now, Instant.MaxValue);

        ArgumentOutOfRangeException.ThrowIfEqual(deploymentNumber, int.MaxValue);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(deploymentNumber);

        var id = new DeploymentId(Guid.NewGuid());
        return new Deployment(
            id,
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

        RaiseDomainEvent(new DeploymentTransient(Id));
    }

    public void SetFailed()
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(State, DeploymentState.Transient);
        State = DeploymentState.Failed;

        RaiseDomainEvent(new DeploymentFailed(Id, "Some error occurs"));
    }

    public void SetDeployed()
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(State, DeploymentState.Transient);
        State = DeploymentState.Deployed;

        RaiseDomainEvent(new DeploymentDeployed(Id));
    }

    public void SetDecommissioned()
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(State, DeploymentState.Deployed);
        State = DeploymentState.Decommissioned;

        RaiseDomainEvent(new DeploymentDecommissioned(Id, "User stop or new deployment in transient"));
    }
}
