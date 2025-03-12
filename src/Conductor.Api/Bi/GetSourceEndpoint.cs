using Bi.Contracts.Enums;
using Bi.Contracts.GetSource;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using NodaTime;
using static Conductor.Api.Bi.GetSourceEndpoint;

namespace Conductor.Api.Bi;

public sealed class GetSourceEndpoint :
    EndpointWithoutRequest<EndpointResponse>
{
    private readonly IMediator _mediator;

    public GetSourceEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/sources/{id}");
        AllowAnonymous();

        Description(x =>
        {
            x.WithDescription("Get data source details");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var command = new GetSourceQuery(SourceId: id);
        var responseOrError = await _mediator.Send(command, ct);
        var response = responseOrError.ValueOrThrow();
        
        await SendAsync(new EndpointResponse(
            SourceId: response.SourceId,
            Name: response.Name,
            PrivateNotes: response.PrivateNotes,
            Description: response.Description,
            ConnectionString: response.ConnectionString,
            Schema: response.Schema,
            State: response.State,
            StateChanged: response.StateChanged));
    }
    
    public sealed record EndpointResponse(
        Guid SourceId,
        string Name,
        string PrivateNotes,
        string Description,
        string ConnectionString,
        string Schema,
        SourceState State,
        Instant StateChanged);
}
