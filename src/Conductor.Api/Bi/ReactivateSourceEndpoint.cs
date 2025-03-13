using Bi.Contracts.ReactivateSource;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using static Conductor.Api.Bi.ReactivateSourceEndpoint;

namespace Conductor.Api.Bi;

public sealed class ReactivateSourceEndpoint : Endpoint<ReactivateSourceRequest>
{
    private readonly IMediator _mediator;

    public ReactivateSourceEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/sources/{id}/reactivate");
        AllowAnonymous();

        Description(x =>
        {
            x.WithDescription("Reactivate data source");
        });
    }

    public override async Task HandleAsync(
        ReactivateSourceRequest request,
        CancellationToken ct)
    {
        var id = Route<Guid>("id");

        var command = new ReactivateSourceCommand(
            SourceId: id,
            Version: request.Version);

        var successOrError = await _mediator.Send(command, ct);
        _ = successOrError.ValueOrThrow();
    }

    public record ReactivateSourceRequest(uint Version);
}
