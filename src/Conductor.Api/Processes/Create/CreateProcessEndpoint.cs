using Conductor.Application.Processes;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Conductor.Api.Processes.Create;

/// <summary>
/// Create a new process with draft and return it.
/// </summary>
internal class CreateProcessEndpoint : Endpoint<
    CreateProcessRequest,
    CreateProcessResponse>
{
    private readonly IMediator _mediator;

    public CreateProcessEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        // TODO: use constants
        Post("/process");
        AllowAnonymous();
        Summary(s =>
        {
            // XML Docs are used by default but are overridden by these properties:
            //s.Summary = "Create a new Contributor.";
            //s.Description = "Create a new Contributor. A valid name is required.";
            s.ExampleRequest = new CreateProcessRequest(
                Name: "myUniqueName",
                DisplayName: "Some disp N123",
                Description: string.Empty);
        });
    }

    // TODO: fluent validate request (just ) AND domain errors by exceptions
    // TODO: use complex-monade result
    public override async Task<CreateProcessResponse> ExecuteAsync(
        CreateProcessRequest req, CancellationToken ct)
    {
        // TODO: use auto mapping
        // TODO: provide auth props (will be checked at app layer)
        var command = new CreateProcessCommand(
            req.Name,
            req.DisplayName,
            req.Description);

        var process = await _mediator.Send(command);
        var response = new CreateProcessResponse(
            process.Id.Id,
            process.Name,
            process.DisplayName,
            process.Description,
            process.Number);

        return response;
    }
}
