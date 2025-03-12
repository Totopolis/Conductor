using Bi.Application.Abstractions;
using Bi.Domain.Abstractions;
using Bi.Domain.Events;
using Bi.Domain.Sources;
using Domain.Shared;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bi.Application.Consumers;

internal sealed class ReactivateSourceConsumer :
    IConsumer<ReactivateSource>
{
    private readonly ISourceRepository _sourceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;
    private readonly IKeyedServiceProvider _serviceProvider;
    private readonly ILogger<ReactivateSourceConsumer> _logger;

    public ReactivateSourceConsumer(
        ISourceRepository sourceRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider,
        IKeyedServiceProvider serviceProvider,
        ILogger<ReactivateSourceConsumer> logger)
    {
        _sourceRepository = sourceRepository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
        _serviceProvider = serviceProvider;
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

        var link = _serviceProvider
            .GetRequiredKeyedService<ISourceLink>(source.Kind.Name);

        // 1. Check connection string and try connect to database
        var successOrError = await link.CheckConnection(
            source.ConnectionString,
            context.CancellationToken);

        if (successOrError.IsError)
        {
            source.Disable(now);
            await _unitOfWork.SaveChanges(context.CancellationToken);
            return;
        }

        // 2. Check schema if need
        // 3. Ask ai about source config (aiNotes)
        source.Enable(now);

        await _unitOfWork.SaveChanges(context.CancellationToken);
    }
}
