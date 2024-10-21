using LangModel.Abstractions.Common;
using LangModel.Abstractions.Diagnostics;
using LangModel.Abstractions.Errors;
using LangModel.Abstractions.Vectorizer;
using MathNet.Numerics.LinearAlgebra;
using OpenAI.Interfaces;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using System.Collections.Immutable;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace LangModel.OpenAi.Vectorizer;

internal sealed class VectorizerService : IVectorizerService
{
    public const decimal Incoming1kCost = 0.03744m;

    private readonly TracerComposite _tracerComposite;
    private readonly IOpenAIService _ai;

    public VectorizerService(
        TracerComposite tracerComposite,
        IOpenAIService ai)
    {
        _tracerComposite = tracerComposite;
        _ai = ai;
    }

    public async Task<VectorizeResponse> Vectorize(
        VectorizeRequest request,
        CancellationToken ct)
    {
        var vectorizerStart = DateTime.Now;

        var totalTokens = 0;
        var zz = new List<Vector<double>>();
        var xx = request.Content
            .Chunk(100)
            .ToList();

        foreach (var it in xx)
        {
            var stepStart = DateTime.Now;
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

            var stepUsage = UsageValue.CreateSingleVectorizer(
                tokenCount: embeddindResponse.Usage.TotalTokens,
                cost1kTokens: Incoming1kCost,
                span: DateTime.Now - stepStart);

            var vectors = embeddindResponse.Data
                .OrderBy(x => x.Index)
                .Select(x => Vector<double>.Build.DenseOfArray(x.Embedding.ToArray()));

            zz.AddRange(vectors);

            var serOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                WriteIndented = true
            };

            var requestJson = JsonSerializer.SerializeToElement(it.ToList(), serOptions);
            var responseJson = JsonSerializer.SerializeToElement(vectors.ToList(), serOptions);

            await _tracerComposite.Trace(
                kind: LangModelTracerKind.Embedding,
                request: requestJson,
                response: responseJson,
                usage: stepUsage);

            // TODO: check total is correct!!!
            totalTokens += embeddindResponse.Usage.TotalTokens;
        }

        var response = new VectorizeResponse(
            Embeddings: zz.ToImmutableArray(),
            Usage: UsageValue.CreateSingleVectorizer(
                tokenCount: totalTokens,
                cost1kTokens: Incoming1kCost,
                span: DateTime.Now - vectorizerStart));

        return response;
    }
}
