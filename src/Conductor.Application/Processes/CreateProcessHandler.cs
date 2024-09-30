using Conductor.Application.Processes;
using Conductor.Domain.Processes;
using MediatR;

namespace Conductor.Application.ApiHandlers;

internal class CreateProcessHandler : IRequestHandler<CreateProcessCommand, Process>
{
    public CreateProcessHandler()
    {
    }

    public Task<Process> Handle(
        CreateProcessCommand request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
