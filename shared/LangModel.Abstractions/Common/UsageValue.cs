namespace LangModel.Abstractions.Common;

public sealed class UsageValue
{
    public required TokenCountPrice Prompt { get; init; }

    public required TokenCountPrice Completion { get; init; }

    public required TokenCountPrice Vectorizer { get; init; }

    public decimal TotalPrice =>
        Prompt.Price + Completion.Price + Vectorizer.Price;

    public required RunCountDuration CompleteModel { get; init; }

    public required RunCountDuration EmbeddingModel { get; init; }

    public required RunCountDuration Tools { get; init; }

    public static readonly UsageValue Empty = new UsageValue
    {
        Prompt = TokenCountPrice.Empty,
        Completion = TokenCountPrice.Empty,
        Vectorizer = TokenCountPrice.Empty,
        CompleteModel = RunCountDuration.Empty,
        EmbeddingModel = RunCountDuration.Empty,
        Tools = RunCountDuration.Empty
    };

    public static UsageValue CreateSingleVectorizer(
        int tokenCount,
        decimal cost1kTokens,
        TimeSpan span)
    {
        return new UsageValue
        {
            Prompt = TokenCountPrice.Empty,
            Completion = TokenCountPrice.Empty,
            Vectorizer = TokenCountPrice.Create(
                tokenCount,
                tokenCount * cost1kTokens / 1000m),
            CompleteModel = RunCountDuration.Empty,
            EmbeddingModel = RunCountDuration.Create(1, span),
            Tools = RunCountDuration.Empty
        };
    }

    public static UsageValue CreateSingleAnswerizer(
        int promptTokens,
        decimal prompt1kCost,
        int? completionTokens,
        decimal completion1kCost,
        TimeSpan span)
    {
        return new UsageValue
        {
            Prompt = TokenCountPrice.Create(
                promptTokens,
                promptTokens * prompt1kCost / 1000m),
            Completion = TokenCountPrice.Create(
                completionTokens ?? 0,
                (completionTokens ?? 0) * completion1kCost / 1000m),
            CompleteModel = RunCountDuration.Create(1, span),
            EmbeddingModel = RunCountDuration.Empty,
            Tools = RunCountDuration.Empty,
            Vectorizer = TokenCountPrice.Empty
        };
    }

    public static UsageValue CreateSingleTool(TimeSpan span)
    {
        return new UsageValue
        {
            Prompt = TokenCountPrice.Empty,
            Completion = TokenCountPrice.Empty,
            CompleteModel = RunCountDuration.Empty,
            Tools = RunCountDuration.Create(1, span),
            EmbeddingModel = RunCountDuration.Empty,
            Vectorizer = TokenCountPrice.Empty
        };
    }

    public static UsageValue operator +(
        UsageValue left,
        UsageValue right)
    {
        return new UsageValue
        {
            Prompt = left.Prompt + right.Prompt,
            Completion = left.Completion + right.Completion,
            CompleteModel = left.CompleteModel + right.CompleteModel,
            EmbeddingModel = left.EmbeddingModel + right.EmbeddingModel,
            Tools = left.Tools + right.Tools,
            Vectorizer = left.Vectorizer + right.Vectorizer
        };
    }
}
