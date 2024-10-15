using LangModel.Abstractions.Common;
using LangModel.Abstractions.Errors;
using LangModel.Abstractions.Vectorizer;
using MathNet.Numerics.LinearAlgebra;
using OpenAI.Interfaces;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using System.Collections.Immutable;

namespace LangModel.OpenAi.Vectorizer;

internal sealed class VectorizerService : IVectorizerService
{
    public const decimal Incoming1kCost = 0.03744m;

    private readonly IOpenAIService _ai;

    public VectorizerService(IOpenAIService ai)
    {
        _ai = ai;
    }

    public async Task<VectorizeResponse> Vectorize(
        VectorizeRequest request,
        CancellationToken ct)
    {
        var usage = 0;
        var zz = new List<Vector<double>>();
        var xx = request.Content
            .Chunk(100)
            .ToList();

        foreach (var it in xx)
        {
            var embeddindResponse = await _ai.Embeddings
                    .CreateEmbedding(new EmbeddingCreateRequest()
                    {
                        InputAsList = it.ToList(),
                        Model = Models.TextEmbeddingV3Large
                    }, ct);

            if (!embeddindResponse.Successful)
            {
                throw new VectorizationErrorException(
                    embeddindResponse.Error?.Message ?? string.Empty);
            }

            // TODO: check total is correct!!!
            usage += embeddindResponse.Usage.TotalTokens;

            var vectors = embeddindResponse.Data
                .OrderBy(x => x.Index)
                .Select(x => Vector<double>.Build.DenseOfArray(x.Embedding.ToArray()));

            zz.AddRange(vectors);
        }

        var usageValue = new UsageValue
        {
            Prompt = CountPrice.Empty,
            Completion = CountPrice.Empty,
            Vectorizer = CountPrice.Create(usage, usage * Incoming1kCost / 1000m),
            LangModel = CountDuration.Empty,
            Tools = CountDuration.Empty
        };

        var response = new VectorizeResponse(
            Embeddings: zz.ToImmutableArray(),
            Usage: usageValue);

        return response;
    }
}
