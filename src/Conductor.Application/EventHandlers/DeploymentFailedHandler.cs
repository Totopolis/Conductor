using Conductor.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Conductor.Application.EventHandlers;

internal class DeploymentFailedHandler : IConsumer<DeploymentFailed>
{
    private readonly ILogger<DeploymentFailedHandler> _logger;

    public DeploymentFailedHandler(ILogger<DeploymentFailedHandler> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<DeploymentFailed> context)
    {
        _logger.LogWarning("Deployment {EntityId} deploy failed. Reason: {Reason}",
            context.Message.DeploymentId,
            context.Message.Reason);

        // TODO: if IMaintenanceBot is enabled and notifications enabled then send
        return Task.CompletedTask;
    }
}
