using Bi.Contracts.CreateSource;
using Bi.Domain.Abstractions;
using Bi.Domain.Sources;
using Domain.Shared;
using ErrorOr;
using MediatR;

namespace Bi.Application.Handlers;

public sealed class CreateSourceHandler : IRequestHandler<
    CreateSourceCommand,
    ErrorOr<CreateSourceCommandResponse>>
{
    private readonly ISourceRepository _sourceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public CreateSourceHandler(
        ISourceRepository sourceRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider)
    {
        _sourceRepository = sourceRepository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    public async Task<ErrorOr<CreateSourceCommandResponse>> Handle(
        CreateSourceCommand request,
        CancellationToken cancellationToken)
    {
        var sourceOrError = Source.CreateNew(
            kind: request.Kind,
            name: request.Name,
            userNotes: request.UserNotes,
            description: request.Description,
            connectionString: request.ConnectionString,
            schema: request.Schema,
            now: _timeProvider.GetInstantNow());

        if (sourceOrError.IsError)
        {
            return sourceOrError.FirstError;
        }

        var source = sourceOrError.Value;

        _sourceRepository.Add(source);

        await _unitOfWork.SaveChanges(cancellationToken);

        return new CreateSourceCommandResponse(
            SourceId: source.Id.Value);
    }
}
