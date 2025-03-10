using Bi.Contracts.Enums;
using ErrorOr;
using MediatR;

namespace Bi.Contracts.CreateDbSource;

public sealed record CreateDbSourceCommand(
    string Kind,
    string Name,
    string PrivateNotes,
    string Description,
    string ConnectionString,
    DbSourceSchemaMode SchemaMode,
    string ManualSchema) : IRequest<ErrorOr<CreateDbSourceCommandResponse>>;
