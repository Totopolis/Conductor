using ErrorOr;
using MediatR;

namespace Lang.Contracts.Ask;

public sealed record AskCommand(
    Guid RequestId,
    AskSequence Sequence,
    bool NeedJson) : IRequest<ErrorOr<AskCommandResult>>;
