using ErrorOr;
using MediatR;

namespace Bi.Contracts.ReactivateSource;

public sealed record ReactivateSourceCommand(
    Guid SourceId,
    uint Version) : IRequest<ErrorOr<Success>>;
