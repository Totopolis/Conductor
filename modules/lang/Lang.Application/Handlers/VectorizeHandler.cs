using Application.Shared.Abstractions;
using Domain.Shared;
using ErrorOr;
using Lang.Application.Abstractions;
using Lang.Contracts.IntegrationEvents;
using Lang.Contracts.Vectorize;
using MediatR;
using ZiggyCreatures.Caching.Fusion;

namespace Lang.Application.Handlers;

internal sealed class VectorizeHandler : IRequestHandler<
    VectorizeCommand,
    ErrorOr<VectorizeCommandResult>>
{
    private readonly IVectorizer _vectorizer;
    private readonly IFusionCache _cache;
    private readonly IEventBusPublisher _eventBus;
    private readonly TimeProvider _timeProvider;

    public VectorizeHandler(
        IVectorizer vectorizer,
        IFusionCacheProvider cacheProvider,
        IEventBusPublisher eventBus,
        TimeProvider timeProvider)
    {
        _vectorizer = vectorizer;
        _cache = cacheProvider.GetCache(Constants.VectorizerCacheName);
        _eventBus = eventBus;
        _timeProvider = timeProvider;
    }

    public async Task<ErrorOr<VectorizeCommandResult>> Handle(
        VectorizeCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Content.Trim().Length < 3)
        {
            return Error.Validation(description: "Content too short");
        }

        try
        {
            bool cacheHit = true;

            var cached = await _cache.GetOrSetAsync(
                key: request.Content,
                factory: async ct2 =>
                {
                    var vector = await _vectorizer.Vectorize(request.Content, cancellationToken);

                    cacheHit = false;
                    return vector;
                },
                token: cancellationToken);

            // TODO: use trans outbox with domain layer
            await _eventBus.Publish(new ContentVectorizedEvent(
                RequestId: request.RequestId,
                Timestamp: _timeProvider.GetInstantNow(),
                // TODO: its not always true!!!!!
                TotalDuration: cached.Duration,
                TokensUsage: cached.TokensUsage,
                CacheHit: cacheHit), cancellationToken);

            return new VectorizeCommandResult(
                Vector: cached.Vector,
                Duration: cached.Duration,
                TokensUsage: cached.TokensUsage,
                CacheHit: cacheHit);
        }
        catch (Exception ex)
        {
            return Error.Failure(description: $"Content vectorize error: {ex.Message}");
        }
    }
}
