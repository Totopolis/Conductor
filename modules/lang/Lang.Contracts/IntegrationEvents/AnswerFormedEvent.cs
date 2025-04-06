using NodaTime;

namespace Lang.Contracts.IntegrationEvents;

public sealed record AnswerFormedEvent(
    Guid RequestId,
    Instant Timestamp,
    Duration TotalDuration,
    int PromptTokensCount,
    int CompletionTokensCount);
