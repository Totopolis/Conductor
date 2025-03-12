using ErrorOr;
using MediatR;

namespace Bi.Contracts.CreateSource;

public sealed record CreateSourceCommand(
    string Kind,
    string Name,
    string PrivateNotes,
    string Description,
    string ConnectionString,
    string Schema) : IRequest<ErrorOr<CreateSourceCommandResponse>>;
