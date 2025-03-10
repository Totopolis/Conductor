using Bi.Application.Diagnostics;
using Bi.Contracts.CreateDbSource;
using Bi.Domain.Abstractions;
using Bi.Domain.DataSources;
using Domain.Shared;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bi.Application.Handlers;

public sealed class CreateDataSourceHandler : IRequestHandler<
    CreateDbSourceCommand,
    ErrorOr<CreateDbSourceCommandResponse>>
{
    private readonly IDbSourceRepository _dataSourceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<CreateDataSourceHandler> _logger;

    public CreateDataSourceHandler(
        IDbSourceRepository dataSourceRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider,
        ILogger<CreateDataSourceHandler> logger)
    {
        _dataSourceRepository = dataSourceRepository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task<ErrorOr<CreateDbSourceCommandResponse>> Handle(
        CreateDbSourceCommand request,
        CancellationToken cancellationToken)
    {
        if (!DbSourceSchemaMode.TryFromName(
            request.SchemaMode.ToString(),
            ignoreCase: true,
            out var schemaMode))
        {
            _logger.LogError("Bad DbSourceSchemaMode name {0}", request.SchemaMode.ToString());
            return ApplicationErrors.UnexpectedError;
        }

        var dataSourceOrError = DbSource.CreateNew(
            kind: request.Kind,
            name: request.Name,
            privateNotes: request.PrivateNotes,
            description: request.Description,
            connectionString: request.ConnectionString,
            schemaMode: schemaMode,
            schema: request.ManualSchema,
            now: _timeProvider.GetInstantNow());

        if (dataSourceOrError.IsError)
        {
            return dataSourceOrError.FirstError;
        }

        var dataSource = dataSourceOrError.Value;

        _dataSourceRepository.Add(dataSource);

        await _unitOfWork.SaveChanges(cancellationToken);

        return new CreateDbSourceCommandResponse(
            DataSourceId: dataSource.Id.Value);
    }
}
