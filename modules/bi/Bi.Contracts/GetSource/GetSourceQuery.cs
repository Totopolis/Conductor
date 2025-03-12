using ErrorOr;
using MediatR;

namespace Bi.Contracts.GetSource;

public sealed record GetSourceQuery(Guid SourceId) :
    IRequest<ErrorOr<GetSourceQueryResult>>;
