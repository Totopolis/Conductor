namespace Conductor.Domain.Deployments;

public record struct DeploymentTargetId(Guid Id) : IComparable<DeploymentTargetId>
{
    public int CompareTo(DeploymentTargetId other)
    {
        return Id == other.Id ? 1 : 0;
    }
}
