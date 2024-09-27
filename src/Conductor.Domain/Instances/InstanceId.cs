namespace Conductor.Domain.Processes;

public record struct InstanceId(Guid Id) : IComparable<InstanceId>
{
    public int CompareTo(InstanceId other)
    {
        return Id == other.Id ? 0 : 1;
    }
}
