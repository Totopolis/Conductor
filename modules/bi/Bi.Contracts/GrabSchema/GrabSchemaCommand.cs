﻿using ErrorOr;
using MediatR;

namespace Bi.Contracts.GrabSchema;

public sealed record GrabSchemaCommand(
    Guid SourceId,
    uint Version) : IRequest<ErrorOr<Success>>;
