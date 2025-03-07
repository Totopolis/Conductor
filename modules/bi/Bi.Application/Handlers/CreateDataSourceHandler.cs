using Bi.Contracts.CreateDataSource;
using Bi.Domain.Abstractions;
using Bi.Domain.DataSources;
using ErrorOr;
using MediatR;

namespace Bi.Application.Handlers;

public sealed class CreateDataSourceHandler : IRequestHandler<
    CreateDataSourceCommand,
    ErrorOr<CreateDataSourceCommandResponse>>
{
    private readonly IDataSourceRepository _dataSourceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDataSourceHandler(
        IDataSourceRepository dataSourceRepository,
        IUnitOfWork unitOfWork)
    {
        _dataSourceRepository = dataSourceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<CreateDataSourceCommandResponse>> Handle(
        CreateDataSourceCommand request,
        CancellationToken cancellationToken)
    {
        var dataSourceOrError = DataSource.CreateNew(
            name: request.Name,
            description: request.Description);

        if (dataSourceOrError.IsError)
        {
            return dataSourceOrError.FirstError;
        }

        var dataSource = dataSourceOrError.Value;

        _dataSourceRepository.Add(dataSource);

        await _unitOfWork.SaveChanges(cancellationToken);

        return new CreateDataSourceCommandResponse(
            DataSourceId: dataSource.Id.Value);
    }
}
