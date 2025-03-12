using Bi.Contracts.CreateSource;
using Bi.Domain.Abstractions;
using Bi.Domain.Sources;
using Domain.Shared;
using ErrorOr;
using MediatR;

namespace Bi.Application.Handlers;

public sealed class CreateDataSourceHandler : IRequestHandler<
    CreateSourceCommand,
    ErrorOr<CreateSourceCommandResponse>>
{
    private readonly ISourceRepository _dataSourceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public CreateDataSourceHandler(
        ISourceRepository dataSourceRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider)
    {
        _dataSourceRepository = dataSourceRepository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    public async Task<ErrorOr<CreateSourceCommandResponse>> Handle(
        CreateSourceCommand request,
        CancellationToken cancellationToken)
    {
        var dataSourceOrError = Source.CreateNew(
            kind: request.Kind,
            name: request.Name,
            privateNotes: request.PrivateNotes,
            description: request.Description,
            connectionString: request.ConnectionString,
            schema: request.Schema,
            now: _timeProvider.GetInstantNow());

        if (dataSourceOrError.IsError)
        {
            return dataSourceOrError.FirstError;
        }

        var dataSource = dataSourceOrError.Value;

        _dataSourceRepository.Add(dataSource);

        await _unitOfWork.SaveChanges(cancellationToken);

        return new CreateSourceCommandResponse(
            SourceId: dataSource.Id.Value);
    }
}
