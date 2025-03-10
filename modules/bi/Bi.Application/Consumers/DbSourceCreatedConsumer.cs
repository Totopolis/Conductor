using Bi.Application.Abstractions;
using Bi.Domain.Abstractions;
using Bi.Domain.DataSources;
using Bi.Domain.Events;
using Domain.Shared;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Bi.Application.Consumers;

internal sealed class DbSourceCreatedConsumer : IConsumer<DbSourceCreated>
{
    private readonly IDbSourceRepository _dbSourceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;
    private readonly IPostgresConnector _postgresConnector;
    private readonly ILogger<DbSourceCreatedConsumer> _logger;

    public DbSourceCreatedConsumer(
        IDbSourceRepository dbSourceRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider,
        IPostgresConnector postgresConnector,
        ILogger<DbSourceCreatedConsumer> logger)
    {
        _dbSourceRepository = dbSourceRepository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
        _postgresConnector = postgresConnector;
        _logger = logger;
    }

    // TODO: use polly retry policies
    public async Task Consume(ConsumeContext<DbSourceCreated> context)
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

        // 1. Try connect to database
        var connectOrError = await _postgresConnector.CheckConnection(
            source.ConnectionString,
            context.CancellationToken);

        if (connectOrError.IsError)
        {
            source.ChangeState(DbSourceState.ConnectionFailed, now);
            await _unitOfWork.SaveChanges(context.CancellationToken);
            return;
        }

        // 2. Try download schema if need
        if (source.SchemaMode.IsAuto)
        {
            var schemaOrError = await _postgresConnector.GrabSchema(
                source.ConnectionString,
                context.CancellationToken);

            if (schemaOrError.IsError)
            {
                source.ChangeState(DbSourceState.SchemaNotAvailable, now);
                await _unitOfWork.SaveChanges(context.CancellationToken);
                return;
            }

            source.UpdateSchema(schemaOrError.Value);
        }

        // 3. Check schema if need
        if (source.SchemaMode.IsManual)
        {
            // TODO
        }

        source.ChangeState(DbSourceState.Ready, now);
        await _unitOfWork.SaveChanges(context.CancellationToken);
    }
}
