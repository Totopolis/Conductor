using NodaTime;

namespace Lang.Contracts.Vectorize;

public sealed record VectorizeCommandResult(
    IReadOnlyList<double> Vector,
    Duration Duration,
    int TokensUsage,
    bool CacheHit);
