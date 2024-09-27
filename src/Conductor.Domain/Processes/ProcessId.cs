namespace Conductor.Domain.Processes;

public record struct ProcessId(Guid Id) : IComparable<ProcessId>
{
    public int CompareTo(ProcessId other)
    {
        return Id == other.Id ? 0 : 1;
    }
}
