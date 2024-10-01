using Conductor.Application.Processes;
using FastEndpoints;
using FluentValidation;
using MediatR;

namespace Conductor.Api.Processes.Create;

/// <summary>
/// Create a new process with draft and return it.
/// </summary>
internal class CreateProcessEndpoint : Endpoint<
    CreateProcessRequest,
    CreateProcessResponse>
{
    private readonly IValidator<CreateProcessRequest> _validator;
    private readonly IMediator _mediator;
    private readonly AutoMapper.IMapper _mapper;

    public CreateProcessEndpoint(
        IValidator<CreateProcessRequest> validator,
        IMediator mediator,
        AutoMapper.IMapper mapper)
    {
        _validator = validator;
        _mediator = mediator;
        _mapper = mapper;
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

    // TODO: use complex-monade result
    public override async Task<CreateProcessResponse> ExecuteAsync(
        CreateProcessRequest req,
        CancellationToken ct)
    {
        // Validate is about just structure and types checks.
        var validationResult = _validator.Validate(req);

        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                AddError(error);
            }

            ThrowIfAnyErrors();
        }

        // TODO: provide auth and claims to command,
        // will be additionally checked at the app layer.
        var command = _mapper.Map<CreateProcessCommand>(req);

        var processResult = await _mediator.Send(command);
        if (!processResult.IsSuccess)
        {
            ThrowError(processResult.Error);
        }

        var response = _mapper.Map<CreateProcessResponse>(processResult.Value);
        return response;
    }
}
