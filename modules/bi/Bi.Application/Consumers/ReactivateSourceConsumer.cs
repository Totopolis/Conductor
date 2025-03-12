using Bi.Application.Abstractions;
using Bi.Domain.Abstractions;
using Bi.Domain.Events;
using Bi.Domain.Sources;
using Domain.Shared;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Bi.Application.Consumers;

internal sealed class ReactivateSourceConsumer :
    IConsumer<ReactivateSource>
{
    private readonly ISourceRepository _sourceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;
    private readonly IPostgresConnector _postgresConnector;
    private readonly ILogger<ReactivateSourceConsumer> _logger;

    public ReactivateSourceConsumer(
        ISourceRepository sourceRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider,
        IPostgresConnector postgresConnector,
        ILogger<ReactivateSourceConsumer> logger)
    {
        _sourceRepository = sourceRepository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
        _postgresConnector = postgresConnector;
        _logger = logger;
    }

    // TODO: use polly retry policies
    public async Task Consume(ConsumeContext<ReactivateSource> context)
    {
        var now = _timeProvider.GetInstantNow();
        var msg = context.Message;
        var source = await _sourceRepository.Find(msg.Id, context.CancellationToken);
        if (source is null)
        {
            _logger.LogError("Source id={0} not found", msg.Id);
            return;
        }

        if (source.Kind != SourceKind.Postgres)
        {
            _logger.LogError("Source kind={0} not supported", source.Kind.Name);
            return;
        }

        // 1. Try connect to database
        var successOrError = await _postgresConnector.CheckConnection(
            source.ConnectionString,
            context.CancellationToken);

        if (successOrError.IsError)
        {
            source.Disable(now);
            await _unitOfWork.SaveChanges(context.CancellationToken);
            return;
        }

        // 2. Check schema if need
        // 3. Ask ai about source config
        source.Enable(now);

        await _unitOfWork.SaveChanges(context.CancellationToken);
    }
}
