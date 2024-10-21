using System.Collections.Immutable;

namespace LangModel.Abstractions.Vectorizer;

public class VectorizeRequest
{
    public Guid CorrelationId { get; init; }

    public ImmutableArray<string> Content { get; init; }

    public static VectorizeRequest Create(
        Guid correlationId,
        IEnumerable<string> content)
    {
        // TODO: add invariant - items not empty!!!
        return new VectorizeRequest
        {
            CorrelationId = correlationId,
            Content = content.ToImmutableArray()
        };
    }
};
