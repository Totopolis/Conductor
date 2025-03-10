using ErrorOr;
using MediatR;

namespace Bi.Contracts.CreateDbSource;

public sealed record GetSourcesQuery() :
    IRequest<ErrorOr<GetSourcesQueryResult>>;
