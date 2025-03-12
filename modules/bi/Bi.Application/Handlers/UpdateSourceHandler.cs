using Bi.Application.Diagnostics;
using Bi.Contracts.CreateSource;
using Bi.Domain.Abstractions;
using Bi.Domain.Sources;
using Domain.Shared;
using ErrorOr;
using MediatR;

namespace Bi.Application.Handlers;

public sealed class UpdateSourceHandler : IRequestHandler<
    UpdateSourceCommand,
    ErrorOr<Success>>
{
    private readonly ISourceRepository _sourceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public UpdateSourceHandler(
        ISourceRepository sourceRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider)
    {
        _sourceRepository = sourceRepository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    public async Task<ErrorOr<Success>> Handle(
        UpdateSourceCommand request,
        CancellationToken cancellationToken)
    {
        var now = _timeProvider.GetInstantNow();

        if (!SourceId.TryFrom(request.SourceId, out var sourceId))
        {
            return ApplicationErrors.BadIdFormat;
        }

        var source = await _sourceRepository.Find(sourceId, cancellationToken);
        if (source is null)
        {
            return ApplicationErrors.SourceNotFound;
        }

        var successOrError = source.LockAndUpdate(
            name: request.Name,
            userNotes: request.UserNotes,
            description: request.Description,
            connectionString: request.ConnectionString,
            schema: request.Schema,
            now: now);
        
        if (successOrError.IsError)
        {
            return successOrError;
        }

        await _unitOfWork.SaveChanges(cancellationToken);

        return Result.Success;
    }
}
