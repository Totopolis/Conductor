using Conductor.Domain.Processes;

namespace Conductor.Domain.Abstractions;

public interface IProcessRepository
{
    Task<Process> GetByIdWithAllRevisions(ProcessId processId, CancellationToken ct);

    Task<Process> GetWithOneRevision(ProcessId processId, RevisionId revisionId, CancellationToken ct);
}
