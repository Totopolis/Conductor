namespace Lang.Application.Abstractions;

public interface IVectorizer
{
    Task<VectorizerResult> Vectorize(string content, CancellationToken ct);
}
