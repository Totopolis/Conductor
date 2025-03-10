using Bi.Contracts.Enums;
using ErrorOr;
using MediatR;

namespace Bi.Contracts.CreateDbSource;

public sealed record UpdateDbSourceCommand(
    Guid DbSourceId,
    string Name,
    string PrivateNotes,
    string Description,
    string ConnectionString,
    DbSourceSchemaMode SchemaMode,
    string ManualSchema) : IRequest<ErrorOr<Success>>;
