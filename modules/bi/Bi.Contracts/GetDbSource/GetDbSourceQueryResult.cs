using Bi.Contracts.Enums;
using NodaTime;

namespace Bi.Contracts.GetDbSource;

public sealed record GetDbSourceQueryResult(
    Guid DbSourceId,
    string Name,
    string PrivateNotes,
    string Description,
    string ConnectionString,
    DbSourceSchemaMode SchemaMode,
    string Schema,
    DbSourceState State,
    Instant StateChanged);
