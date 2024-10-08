namespace LangModel.Abstractions.Answerizer;

public record AnswerStat(
    int PromptsTokens,
    decimal PromptsPrice,
    int CompletionsTokens,
    decimal CompletionsPrice,
    decimal TotalSpent,
    int LangModelCalls,
    TimeSpan LangModelDuration,
    int ToolsCalls,
    TimeSpan ToolsDuration);
