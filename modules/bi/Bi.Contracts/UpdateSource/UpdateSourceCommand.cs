using ErrorOr;
using MediatR;

namespace Bi.Contracts.CreateSource;

public sealed record UpdateSourceCommand(
    Guid SourceId,
    string Name,
    string UserNotes,
    string Description,
    string ConnectionString,
    string Schema,
    uint Version) : IRequest<ErrorOr<Success>>;
