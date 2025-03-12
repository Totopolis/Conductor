using Bi.Contracts.Enums;
using NodaTime;

namespace Bi.Contracts.GetSource;

public sealed record GetSourceQueryResult(
    Guid SourceId,
    string Name,
    string PrivateNotes,
    string Description,
    string ConnectionString,
    string Schema,
    SourceState State,
    Instant StateChanged);
