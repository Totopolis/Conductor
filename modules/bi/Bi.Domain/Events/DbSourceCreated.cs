using Bi.Domain.DataSources;
using Domain.Shared;

namespace Bi.Domain.Events;

public sealed record DbSourceCreated(
    DbSourceId Id,
    string Name) : IDomainEvent;
