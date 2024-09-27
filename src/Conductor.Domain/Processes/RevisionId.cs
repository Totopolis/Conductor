namespace Conductor.Domain.Processes;

public record struct RevisionId(Guid Id) : IComparable<RevisionId>
{
    public int CompareTo(RevisionId other)
    {
        return Id == other.Id ? 0 : 1;
    }
}
