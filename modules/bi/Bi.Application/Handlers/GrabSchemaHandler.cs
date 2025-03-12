using Bi.Application.Diagnostics;
using Bi.Contracts.GrabSchema;
using Bi.Domain.Abstractions;
using Bi.Domain.Sources;
using Domain.Shared;
using ErrorOr;
using MediatR;

namespace Bi.Application.Handlers;

public sealed class GrabSchemaHandler : IRequestHandler<
    GrabSchemaCommand,
    ErrorOr<Success>>
{
    private readonly ISourceRepository _sourceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public GrabSchemaHandler(
        ISourceRepository sourceRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider)
    {
        _sourceRepository = sourceRepository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    public async Task<ErrorOr<Success>> Handle(
        GrabSchemaCommand request,
        CancellationToken cancellationToken)
    {
        if (!SourceId.TryFrom(request.SourceId, out var sourceId))
        {
            return ApplicationErrors.BadIdFormat;
        }

        var source = await _sourceRepository.Find(sourceId, cancellationToken);
        if (source is null)
        {
            return ApplicationErrors.SourceNotFound;
        }

        var successOrError = source.LockAndGrabSchema(_timeProvider.GetInstantNow());
        if (successOrError.IsError)
        {
            return successOrError;
        }

        await _unitOfWork.SaveChanges(cancellationToken);

        return Result.Success;
    }
}
