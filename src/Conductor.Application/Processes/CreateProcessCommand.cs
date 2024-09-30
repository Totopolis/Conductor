using Conductor.Domain.Processes;
using MediatR;

namespace Conductor.Application.Processes;

// TODO: use result monade to return
public record CreateProcessCommand(
    string Name,
    string DisplayName,
    string Description) : IRequest<Process>;
