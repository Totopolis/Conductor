using NodaTime;
using static Bi.Contracts.CreateDbSource.GetSourcesQueryResult;

namespace Bi.Contracts.CreateDbSource;

public sealed record GetSourcesQueryResult(
    IReadOnlyList<Source> Sources)
{
    public sealed record Source(
        Guid Id,
        string kind,
        string Name,
        string State,
        Instant StateChanged);
}
