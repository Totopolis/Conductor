using Bi.Contracts.CreateDataSource;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Conductor.Api.Bi;

public sealed class CreateDataSourceEndpoint :
    Endpoint<CreateDataSourceRequest, CreateDataSourceResponse>
{
    private readonly IMediator _mediator;

    public CreateDataSourceEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/datasources");
        Policies(Constants.AllAuthenticsPolicy);

        Description(x =>
        {
            x.WithTags(Constants.UserTagName);
            x.WithDescription("Create new data source");
        });
    }

    public override async Task HandleAsync(
        CreateDataSourceRequest req,
        CancellationToken ct)
    {
        var command = new CreateDataSourceCommand(
            Name: req.Name,
            Description: req.Description);

        var responseOrError = await _mediator.Send(command, ct);
        var response = responseOrError.ValueOrThrow();

        await SendAsync(
            new CreateDataSourceResponse(DataSourceId: response.DataSourceId));
    }
}

public record CreateDataSourceRequest(string Name, string Description);

public record CreateDataSourceResponse(Guid DataSourceId);
