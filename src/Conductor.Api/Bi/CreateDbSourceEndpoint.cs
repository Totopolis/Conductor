using Bi.Contracts.CreateDbSource;
using Bi.Contracts.Enums;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Conductor.Api.Bi;

public sealed class CreateDbSourceEndpoint :
    Endpoint<CreateDbSourceRequest, CreateDbSourceResponse>
{
    private readonly IMediator _mediator;

    public CreateDbSourceEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/datasources");

        Description(x =>
        {
            x.WithDescription("Create new data source");
        });
    }

    public override async Task HandleAsync(
        CreateDbSourceRequest req,
        CancellationToken ct)
    {
        var command = new CreateDbSourceCommand(
            Kind: "Postgres",
            Name: req.Name,
            PrivateNotes: req.PrivateNotes,
            Description: req.Description,
            ConnectionString: req.ConnectionString,
            SchemaMode: req.SchemaMode,
            ManualSchema: req.ManualSchema);

        var responseOrError = await _mediator.Send(command, ct);
        var response = responseOrError.ValueOrThrow();

        await SendAsync(
            new CreateDbSourceResponse(DbSourceId: response.DataSourceId));
    }
}

public record CreateDbSourceRequest(
    string Name,
    string Kind,
    string PrivateNotes,
    string Description,
    string ConnectionString,
    DbSourceSchemaMode SchemaMode,
    string ManualSchema);

public record CreateDbSourceResponse(Guid DbSourceId);
