using NodaTime;

namespace Lang.Contracts.IntegrationEvents;

// TODO: split into cached and not cached
public sealed record ContentVectorizedEvent(
    Guid RequestId,
    Instant Timestamp,
    Duration TotalDuration,
    int TokensUsage,
    bool CacheHit);
