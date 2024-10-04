using Conductor.Application.Diagnostics;
using Conductor.Application.Processes;
using Conductor.Domain;
using Conductor.Domain.Abstractions;
using Conductor.Domain.Processes;
using ErrorOr;
using MediatR;

namespace Conductor.Application.ApiHandlers;

internal class CreateProcessHandler : IRequestHandler<
    CreateProcessCommand,
    ErrorOr<Process>>
{
    private readonly INumberService _numberService;
    private readonly IProcessRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProcessHandler(
        INumberService numberService,
        IProcessRepository repository,
        IUnitOfWork unitOfWork)
    {
        _numberService = numberService;
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Process>> Handle(
        CreateProcessCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var processNumber = await _numberService.GenerateNext(
                INumberService.GeneratorType.Process);

            var result = Process.Create(
                request.Name,
                request.DisplayName,
                request.Description,
                // TODO: use di for time providers
                // TODO: ban "TimeProvider." symbol
                now: TimeProvider.System.GetInstantNow(),
                processNumber);

            if (result.IsError)
            {
                return result;
            }

            _repository.Add(result.Value);
            await _unitOfWork.SaveChanges(cancellationToken);

            return result.Value;
        }
        catch
        {
            return ApplicationErrors.CommandHandlerUnhandledException;
        }
    }
}
