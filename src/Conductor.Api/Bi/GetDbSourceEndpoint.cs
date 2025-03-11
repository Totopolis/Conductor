using Bi.Contracts.Enums;
using Bi.Contracts.GetDbSource;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using NodaTime;
using static Conductor.Api.Bi.GetDbSourceEndpoint;

namespace Conductor.Api.Bi;

public sealed class GetDbSourceEndpoint :
    EndpointWithoutRequest<EndpointResponse>
{
    private readonly IMediator _mediator;

    public GetDbSourceEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/db-sources/{id}");
        AllowAnonymous();

        Description(x =>
        {
            x.WithDescription("Get database source details");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var command = new GetDbSourceQuery(DbSourceId: id);
        var responseOrError = await _mediator.Send(command, ct);
        var response = responseOrError.ValueOrThrow();
        
        await SendAsync(new EndpointResponse(
            DbSourceId: response.DbSourceId,
            Name: response.Name,
            PrivateNotes: response.PrivateNotes,
            Description: response.Description,
            ConnectionString: response.ConnectionString,
            SchemaMode: response.SchemaMode,
            Schema: response.Schema,
            State: response.State,
            StateChanged: response.StateChanged));
    }
    
    public sealed record EndpointResponse(
        Guid DbSourceId,
        string Name,
        string PrivateNotes,
        string Description,
        string ConnectionString,
        DbSourceSchemaMode SchemaMode,
        string Schema,
        DbSourceState State,
        Instant StateChanged);
}
