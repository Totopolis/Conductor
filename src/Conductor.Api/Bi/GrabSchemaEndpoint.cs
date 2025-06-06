﻿using Bi.Contracts.GrabSchema;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using static Conductor.Api.Bi.GrabSchemaEndpoint;

namespace Conductor.Api.Bi;

public sealed class GrabSchemaEndpoint : Endpoint<GrabSchemaRequest>
{
    private readonly IMediator _mediator;

    public GrabSchemaEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/sources/{id}/schema/grab");
        AllowAnonymous();

        Description(x =>
        {
            x.WithDescription("Grab schema of data source");
        });
    }

    public override async Task HandleAsync(
        GrabSchemaRequest request,
        CancellationToken ct)
    {
        var id = Route<Guid>("id");

        var command = new GrabSchemaCommand(
            SourceId: id,
            Version: request.Version);

        var successOrError = await _mediator.Send(command, ct);
        _ = successOrError.ValueOrThrow();
    }

    public record GrabSchemaRequest(uint Version);
}
