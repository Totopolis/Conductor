using Bi.Application.Abstractions;
using Bi.Domain.Abstractions;
using Bi.Domain.DataSources;
using Bi.Domain.Events;
using Domain.Shared;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Bi.Application.Consumers;

internal sealed class NeedUpdateConsumer :
    IConsumer<NeedUpdateDbSource>
{
    private readonly IDbSourceRepository _dbSourceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<NeedUpdateConsumer> _logger;

    public NeedUpdateConsumer(
        IDbSourceRepository dbSourceRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider,
        ILogger<NeedUpdateConsumer> logger)
    {
        _dbSourceRepository = dbSourceRepository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<NeedUpdateDbSource> context)
    {
        var now = _timeProvider.GetInstantNow();
        var msg = context.Message;
        var source = await _dbSourceRepository.Find(msg.Id, context.CancellationToken);
        if (source is null)
        {
            _logger.LogError("DbSource id={0} not found", msg.Id);
            return;
        }

        if (source.Kind != DbSourceKind.Postgres)
        {
            _logger.LogError("DbSource kind={0} not supported", source.Kind.Name);
            return;
        }

        source.UpdateDefinition(
            name: msg.Name,
            privateNotes: msg.PrivateNotes,
            description: msg.Description,
            connectionString: msg.ConnectionString,
            schemaMode: msg.SchemaMode,
            schema: msg.ManualSchema);

        source.SetState(DbSourceState.Setup, now);
        await _unitOfWork.SaveChanges(context.CancellationToken);
    }
}
