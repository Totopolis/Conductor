using Bi.Domain.DataSources;

namespace Bi.Domain.Abstractions;

public interface IDbSourceRepository
{
    void Add(DbSource dataSource);

    Task<DbSource?> Find(DbSourceId id, CancellationToken ct);

    Task<IReadOnlyList<DbSource>> GetAll(CancellationToken ct);
}
