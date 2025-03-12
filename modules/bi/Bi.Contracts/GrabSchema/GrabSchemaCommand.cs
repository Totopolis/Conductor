using ErrorOr;
using MediatR;

namespace Bi.Contracts.GrabSchema;

public sealed record GrabSchemaCommand(
    Guid SourceId) : IRequest<ErrorOr<Success>>;
