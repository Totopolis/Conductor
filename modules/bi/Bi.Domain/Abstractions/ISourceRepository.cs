using Bi.Domain.Sources;

namespace Bi.Domain.Abstractions;

public interface ISourceRepository
{
    void Add(Source dataSource);

    Task<Source?> Find(SourceId id, CancellationToken ct);

    Task<IReadOnlyList<Source>> GetAll(CancellationToken ct);
}
