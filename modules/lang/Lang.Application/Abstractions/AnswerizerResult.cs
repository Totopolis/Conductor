using NodaTime;

namespace Lang.Application.Abstractions;

public sealed record AnswerizerResult(
    string Answer,
    Duration Duration,
    int PromptTokensUsage,
    int CompletionTokensUsage);
