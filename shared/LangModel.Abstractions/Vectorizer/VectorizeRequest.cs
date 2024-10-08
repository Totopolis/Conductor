using System.Collections.Immutable;

namespace LangModel.Abstractions.Vectorizer;

public class VectorizeRequest
{
    public ImmutableArray<string> Content { get; init; }

    public static VectorizeRequest Create(IEnumerable<string> content)
    {
        // TODO: add invariant - items not empty!!!
        return new VectorizeRequest
        {
            Content = content.ToImmutableArray()
        };
    }
};
