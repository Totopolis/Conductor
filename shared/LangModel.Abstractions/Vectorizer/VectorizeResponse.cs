using MathNet.Numerics.LinearAlgebra;
using System.Collections.Immutable;

namespace LangModel.Abstractions.Vectorizer;

public record VectorizeResponse(
    ImmutableArray<Vector<double>> Embeddings,
    int TokensUsage);
