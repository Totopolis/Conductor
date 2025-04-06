namespace Lang.Application.Abstractions;

public interface IAnswerizer
{
    Task<AnswerizerResult> CompleteSequence(
        string prompt,
        IReadOnlyList<string> userQuestions,
        bool needJson,
        CancellationToken ct);
}
