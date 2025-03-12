﻿using ErrorOr;
using MediatR;

namespace Bi.Contracts.CreateSource;

public sealed record UpdateSourceCommand(
    Guid SourceId,
    string Name,
    string PrivateNotes,
    string Description,
    string ConnectionString,
    string Schema) : IRequest<ErrorOr<Success>>;
