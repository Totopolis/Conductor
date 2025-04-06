using NodaTime;

namespace Lang.Contracts.Ask;

public sealed record AskCommandResult(
    bool IsJson,
    string Answer,
    Duration Duration,
    int PromptTokensUsage,
    int CompletionTokensCount,
    bool CacheHit);
