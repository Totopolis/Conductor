using Bi.Contracts.CreateSource;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using static Conductor.Api.Bi.UpdateSourceEndpoint;

namespace Conductor.Api.Bi;

public sealed class UpdateSourceEndpoint :
    Endpoint<UpdateSourceRequest>
{
    private readonly IMediator _mediator;

    public UpdateSourceEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("/sources/{id}");
        AllowAnonymous();

        Description(x =>
        {
            x.WithDescription("Update data source");
        });
    }

    public override async Task HandleAsync(
        UpdateSourceRequest req,
        CancellationToken ct)
    {
        var id = Route<Guid>("id");

        var command = new UpdateSourceCommand(
            SourceId: id,
            Name: req.Name,
            UserNotes: req.UserNotes,
            Description: req.Description,
            ConnectionString: req.ConnectionString,
            Schema: req.Schema,
            Version: req.Version);

        var successOrError = await _mediator.Send(command, ct);
        _ = successOrError.ValueOrThrow();
    }

    public record UpdateSourceRequest(
        string Name,
        string Kind,
        string UserNotes,
        string Description,
        string ConnectionString,
        string Schema,
        uint Version);
}
