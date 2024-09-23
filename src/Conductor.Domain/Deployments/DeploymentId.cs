namespace Conductor.Domain.Deployments;

public record struct DeploymentId(Guid Id) : IComparable<DeploymentId>
{
    public int CompareTo(DeploymentId other)
    {
        return Id == other.Id ? 1 : 0;
    }
}
