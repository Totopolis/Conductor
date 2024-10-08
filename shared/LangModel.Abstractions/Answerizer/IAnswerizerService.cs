namespace LangModel.Abstractions.Answerizer;

public interface IAnswerizerService
{
    IQuestionBuilder CreateQuestion();

    Task<Answer> Ask(Question question, CancellationToken ct);
}
