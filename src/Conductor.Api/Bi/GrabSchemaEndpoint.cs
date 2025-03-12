using Bi.Contracts.GrabSchema;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Conductor.Api.Bi;

public sealed class GrabSchemaEndpoint : EndpointWithoutRequest
{
    private readonly IMediator _mediator;

    public GrabSchemaEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/sources/{id}/schema/grab");
        AllowAnonymous();

        Description(x =>
        {
            x.WithDescription("Grab schema of data source");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");

        var command = new GrabSchemaCommand(
            SourceId: id);

        var successOrError = await _mediator.Send(command, ct);
        _ = successOrError.ValueOrThrow();
    }
}
