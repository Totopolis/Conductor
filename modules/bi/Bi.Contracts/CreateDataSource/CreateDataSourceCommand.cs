using ErrorOr;
using MediatR;

namespace Bi.Contracts.CreateDataSource;

public sealed record CreateDataSourceCommand(
    string Name,
    string Description) : IRequest<ErrorOr<CreateDataSourceCommandResponse>>;
