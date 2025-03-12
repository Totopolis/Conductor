using ErrorOr;
using MediatR;

namespace Bi.Contracts.CreateSource;

public sealed record GetSourcesQuery() :
    IRequest<ErrorOr<GetSourcesQueryResult>>;
