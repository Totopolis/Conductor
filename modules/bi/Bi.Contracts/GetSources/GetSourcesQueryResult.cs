using NodaTime;
using static Bi.Contracts.CreateSource.GetSourcesQueryResult;

namespace Bi.Contracts.CreateSource;

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
