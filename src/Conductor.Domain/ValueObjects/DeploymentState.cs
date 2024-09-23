using CSharpFunctionalExtensions;

namespace Conductor.Domain.ValueObjects;

public sealed class DeploymentState : 
    EnumValueObject<DeploymentState>,
    IEquatable<DeploymentState>
{
    private DeploymentState(string id) : base(id)
    {
    }

    public static DeploymentState Draft = new("draft");

    public static DeploymentState Transient = new("transient");

    public static DeploymentState Failed = new("failed");

    public static DeploymentState Deployed = new("deployed");

    public static DeploymentState Decommissioned = new("decommissioned");

    public bool Equals(DeploymentState? other)
    {
        return this == other;
    }
}
