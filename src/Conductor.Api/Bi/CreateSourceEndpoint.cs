﻿using Bi.Contracts.CreateSource;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using static Conductor.Api.Bi.CreateSourceEndpoint;

namespace Conductor.Api.Bi;

public sealed class CreateSourceEndpoint :
    Endpoint<CreateSourceRequest, CreateSourceResponse>
{
    private readonly IMediator _mediator;

    public CreateSourceEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/sources");
        AllowAnonymous();

        Description(x =>
        {
            x.WithDescription("Create new data source");
        });
    }

    public override async Task HandleAsync(
        CreateSourceRequest req,
        CancellationToken ct)
    {
        var command = new CreateSourceCommand(
            Kind: req.Kind,
            Name: req.Name,
            UserNotes: req.UserNotes,
            Description: req.Description,
            ConnectionString: req.ConnectionString,
            Schema: req.Schema);

        var responseOrError = await _mediator.Send(command, ct);
        var response = responseOrError.ValueOrThrow();

        await SendAsync(
            new CreateSourceResponse(SourceId: response.SourceId));
    }

    public record CreateSourceRequest(
        string Kind,
        string Name,
        string UserNotes,
        string Description,
        string ConnectionString,
        string Schema);

    public record CreateSourceResponse(Guid SourceId);
}
