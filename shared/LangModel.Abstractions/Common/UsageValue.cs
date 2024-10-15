namespace LangModel.Abstractions.Common;

public sealed class UsageValue
{
    public required CountPrice Prompt { get; init; }

    public required CountPrice Completion { get; init; }

    public required CountPrice Vectorizer { get; init; }

    public decimal TotalPrice =>
        Prompt.Price + Completion.Price + Vectorizer.Price;

    public required CountDuration LangModel { get; init; }

    public required CountDuration Tools { get; init; }

    public static readonly UsageValue Empty = new UsageValue
    {
        Prompt = CountPrice.Empty,
        Completion = CountPrice.Empty,
        Vectorizer = CountPrice.Empty,
        LangModel = CountDuration.Empty,
        Tools = CountDuration.Empty
    };

    public static UsageValue CreateSingleTool(TimeSpan span)
    {
        return new UsageValue
        {
            Prompt = CountPrice.Empty,
            Completion = CountPrice.Empty,
            LangModel = CountDuration.Empty,
            Tools = CountDuration.Create(1, span),
            Vectorizer = CountPrice.Empty
        };
    }

    public static UsageValue operator +(UsageValue left, UsageValue right)
    {
        return new UsageValue
        {
            Prompt = left.Prompt + right.Prompt,
            Completion = left.Completion + right.Completion,
            LangModel = left.LangModel + right.LangModel,
            Tools = left.Tools + right.Tools,
            Vectorizer = left.Vectorizer + right.Vectorizer
        };
    }
}
