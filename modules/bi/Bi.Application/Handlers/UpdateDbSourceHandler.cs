using Bi.Application.Diagnostics;
using Bi.Contracts.CreateDbSource;
using Bi.Domain.Abstractions;
using Bi.Domain.DataSources;
using Domain.Shared;
using ErrorOr;
using MediatR;

namespace Bi.Application.Handlers;

public sealed class UpdateDbSourceHandler : IRequestHandler<
    UpdateDbSourceCommand,
    ErrorOr<Success>>
{
    private readonly IDbSourceRepository _dbSourceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public UpdateDbSourceHandler(
        IDbSourceRepository dbSourceRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider)
    {
        _dbSourceRepository = dbSourceRepository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    public async Task<ErrorOr<Success>> Handle(
        UpdateDbSourceCommand request,
        CancellationToken cancellationToken)
    {
        var now = _timeProvider.GetInstantNow();

        if (!DbSourceId.TryFrom(request.DbSourceId, out var sourceId))
        {
            return ApplicationErrors.BadIdFormat;
        }

        var source = await _dbSourceRepository.Find(sourceId, cancellationToken);
        if (source is null)
        {
            return ApplicationErrors.DbSourceNotFound;
        }

        if (source.State.IsSetup)
        {
            return ApplicationErrors.DbSourceBusy;
        }

        var successOrError = source.RaiseNeedUpdateEventOrError(
            name: request.Name,
            privateNotes: request.PrivateNotes,
            description: request.Description,
            connectionString: request.ConnectionString,
            schemaMode: request.SchemaMode.ToString(),
            schema: request.ManualSchema,
            now: now);
        
        if (successOrError.IsError)
        {
            return successOrError;
        }

        await _unitOfWork.SaveChanges(cancellationToken);

        return Result.Success;
    }
}
