using Bi.Domain.Abstractions;
using Bi.Domain.Events;
using Bi.Domain.Sources;
using Domain.Shared;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Bi.Application.Consumers;

internal sealed class NeedUpdateConsumer :
    IConsumer<NeedUpdateSource>
{
    private readonly ISourceRepository _sourceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<NeedUpdateConsumer> _logger;

    public NeedUpdateConsumer(
        ISourceRepository sourceRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider,
        ILogger<NeedUpdateConsumer> logger)
    {
        _sourceRepository = sourceRepository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<NeedUpdateSource> context)
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

        source.UpdateDefinition(
            name: msg.Name,
            privateNotes: msg.PrivateNotes,
            description: msg.Description,
            connectionString: msg.ConnectionString,
            schema: msg.Schema);

        source.SetState(SourceState.Setup, now);
        await _unitOfWork.SaveChanges(context.CancellationToken);
    }
}
