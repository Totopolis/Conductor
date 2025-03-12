using Bi.Contracts.CreateSource;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using NodaTime;
using static Conductor.Api.Bi.GetSourcesEndpoint;

namespace Conductor.Api.Bi;

// TODO: use fields shaping
public sealed class GetSourcesEndpoint :
    EndpointWithoutRequest<GetSourcesResponse>
{
    private readonly IMediator _mediator;

    public GetSourcesEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/sources");
        AllowAnonymous();

        Description(x =>
        {
            x.WithDescription("Fetch sources");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var command = new GetSourcesQuery();
        var responseOrError = await _mediator.Send(command, ct);
        var response = responseOrError.ValueOrThrow();

        var sources = response.Sources
            .Select(x => new SourceDefinition(
                Id: x.Id,
                Kind: x.kind,
                Name: x.Name,
                State: x.State,
                StateChanged: x.StateChanged))
            .ToList()
            .AsReadOnly();

        await SendAsync(new GetSourcesResponse(Sources: sources));
    }
    
    public record GetSourcesResponse(IReadOnlyList<SourceDefinition> Sources);

    public sealed record SourceDefinition(
        Guid Id,
        string Kind,
        string Name,
        string State,
        Instant StateChanged);
}
