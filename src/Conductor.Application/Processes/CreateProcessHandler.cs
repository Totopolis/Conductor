using Conductor.Application.Processes;
using Conductor.Domain;
using Conductor.Domain.Abstractions;
using Conductor.Domain.Processes;
using CSharpFunctionalExtensions;
using MediatR;

namespace Conductor.Application.ApiHandlers;

internal class CreateProcessHandler : IRequestHandler<CreateProcessCommand, Result<Process>>
{
    private readonly INumberService _numberService;

    public CreateProcessHandler(INumberService numberService)
    {
        _numberService = numberService;
    }

    public async Task<Result<Process>> Handle(
        CreateProcessCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var processNumber = await _numberService.GenerateNext(
                INumberService.GeneratorType.Process);

            // TODO: use result pattern in the factory method instead exceptions
            var process = Process.Create(
                request.Name,
                request.DisplayName,
                request.Description,
                // TODO: use di for time providers
                // TODO: ban "TimeProvider." symbol
                now: TimeProvider.System.GetInstantNow(),
                processNumber);

            return process;
        }
        catch (Exception ex)
        {
            return Result.Failure<Process>(ex.Message);
        }
    }
}
