using Bi.Contracts.Enums;
using NodaTime;

namespace Bi.Contracts.GetSource;

public sealed record GetSourceQueryResult(
    Guid SourceId,
    string Name,
    string UserNotes,
    string Description,
    string ConnectionString,
    string Schema,
    string AiNotes,
    SourceState State,
    Instant StateChanged);
