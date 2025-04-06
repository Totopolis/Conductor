using ErrorOr;
using MediatR;

namespace Lang.Contracts.Vectorize;

public sealed record VectorizeCommand(
    Guid RequestId,
    string Content) : IRequest<ErrorOr<VectorizeCommandResult>>;
