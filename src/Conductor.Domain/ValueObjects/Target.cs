using Conductor.Domain.Processes;

namespace Conductor.Domain.ValueObjects;

public sealed class Target
{
    private Target()
    {
    }

    public ProcessId ProcessId { get; init; }

    public RevisionId RevisionId { get; init; }

    public int ParallelCount { get; init; }

    public int BufferSize { get; init; }

    public static Target Create(
        ProcessId processId,
        RevisionId revisionId,
        int parallelCount,
        int bufferSize)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(processId.Id, Guid.Empty);
        ArgumentOutOfRangeException.ThrowIfEqual(revisionId.Id, Guid.Empty);

        ArgumentOutOfRangeException.ThrowIfNegative(parallelCount);
        ArgumentOutOfRangeException.ThrowIfNegative(bufferSize);

        return new Target
        {
            ProcessId = processId,
            RevisionId = revisionId,
            ParallelCount = parallelCount,
            BufferSize = bufferSize
        };
    }
}
