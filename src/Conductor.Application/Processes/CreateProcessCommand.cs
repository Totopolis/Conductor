using Conductor.Domain.Processes;
using ErrorOr;
using MediatR;

namespace Conductor.Application.Processes;

public record CreateProcessCommand(
    string Name,
    string DisplayName,
    string Description) : IRequest<ErrorOr<Process>>;
