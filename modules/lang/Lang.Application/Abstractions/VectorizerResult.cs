using NodaTime;

namespace Lang.Application.Abstractions;

public sealed record VectorizerResult(
    IReadOnlyList<double> Vector,
    Duration Duration,
    int TokensUsage);
