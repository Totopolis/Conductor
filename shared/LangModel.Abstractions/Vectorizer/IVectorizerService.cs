namespace LangModel.Abstractions.Vectorizer;

public interface IVectorizerService
{
    Task<VectorizeResponse> Vectorize(VectorizeRequest request, CancellationToken ct);
}
