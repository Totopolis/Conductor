using MathNet.Numerics.LinearAlgebra;
using System.Collections.Immutable;

namespace LangModel.Abstractions.Vectorize;

public record VectorizeResponse(
    ImmutableArray<Vector<double>> Embeddings,
    int TokensUsage);
