using Bi.Domain.Abstractions;
using Bi.Domain.Events;
using Domain.Shared;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Bi.Application.Consumers;

internal sealed class UpdateSourceConsumer :
    IConsumer<UpdateSource>
{
    private readonly ISourceRepository _sourceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<UpdateSourceConsumer> _logger;

    public UpdateSourceConsumer(
        ISourceRepository sourceRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider,
        ILogger<UpdateSourceConsumer> logger)
    {
        _sourceRepository = sourceRepository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UpdateSource> context)
    {
        var now = _timeProvider.GetInstantNow();
        var msg = context.Message;
        var source = await _sourceRepository.Find(msg.Id, context.CancellationToken);
        if (source is null)
        {
            _logger.LogError("Source id={0} not found", msg.Id);
            return;
        }

        if (source.State.IsLock)
        {
            _logger.LogError("Source id={0} busy now. Update rejected.", source.Id);
            // TODO: may be signalR to client?
            return;
        }

        source.UpdateDefinition(
            name: msg.Name,
            userNotes: msg.UserNotes,
            description: msg.Description,
            connectionString: msg.ConnectionString,
            schema: msg.Schema);

        source.Disable(now);

        await _unitOfWork.SaveChanges(context.CancellationToken);
    }
}
