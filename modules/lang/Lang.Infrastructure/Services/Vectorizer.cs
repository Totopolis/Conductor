using Betalgo.Ranul.OpenAI.Interfaces;
using Betalgo.Ranul.OpenAI.ObjectModels;
using Betalgo.Ranul.OpenAI.ObjectModels.RequestModels;
using Lang.Application.Abstractions;
using NodaTime;
using System.Diagnostics;

namespace Lang.Infrastructure.Services;

internal sealed class Vectorizer : IVectorizer
{
    private readonly IOpenAIService _ai;

    public Vectorizer(IOpenAIService ai)
    {
        _ai = ai;
    }

    public async Task<VectorizerResult> Vectorize(string content, CancellationToken ct)
    {
        var stopWatch = Stopwatch.StartNew();

        var embeddindResponse = await _ai.Embeddings
            .CreateEmbedding(new EmbeddingCreateRequest()
            {
                Input = content,
                Model = Models.TextEmbeddingV3Large
            }, ct);

        if (!embeddindResponse.Successful)
        {
            throw new InvalidOperationException(
                message: embeddindResponse.Error?.Message ?? string.Empty);
        }

        stopWatch.Stop();

        var embedding = embeddindResponse.Data.FirstOrDefault();
        if (embedding is null)
        {
            throw new InvalidOperationException(
                message: "No vectorized data returned");
        }

        return new VectorizerResult(
            Vector: embedding.Embedding,
            Duration: Duration.FromTimeSpan(stopWatch.Elapsed),
            TokensUsage: embeddindResponse.Usage.PromptTokens);
    }
}
