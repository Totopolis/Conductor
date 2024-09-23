using LangModel.Abstractions.Vectorize;

namespace LangModel.Abstractions;

public interface ILangModel
{
    IQuestionBuilder CreateQuestion();

    Task<Answer> Ask(Question question, CancellationToken ct);

    Task<VectorizeResponse> Vectorize(VectorizeRequest request, CancellationToken ct);
}
