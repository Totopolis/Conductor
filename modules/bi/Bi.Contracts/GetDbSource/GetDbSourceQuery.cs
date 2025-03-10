using Bi.Contracts.CreateDbSource;
using ErrorOr;
using MediatR;

namespace Bi.Contracts.GetDbSource;

public sealed record GetDbSourceQuery(Guid DbSourceId) :
    IRequest<ErrorOr<GetDbSourceQueryResult>>;
