using Conductor.Domain.Abstractions;
using Conductor.Domain.Primitives;
using Conductor.Infrastructure.EventBus;

namespace Conductor.Infrastructure.Database;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly IEventBus _eventBus;
    private readonly ConductorDbContext _dbContext;

    public UnitOfWork(IEventBus eventBus, ConductorDbContext dbContext)
    {
        _eventBus = eventBus;
        _dbContext = dbContext;
    }

    public async Task SaveChanges(CancellationToken ct = default)
    {
        var _ = await _dbContext.SaveChangesAsync(ct);
        
        await PublishDomainEvents(ct);
    }

    // TODO: ConvertDomainEventsToOutboxMessages - use transactional outbox pattern
    // instead delayed send domain events.
    private async Task PublishDomainEvents(CancellationToken ct)
    {
        var events = ExtractDomainEvents().ToList();

        if (events.Count == 0)
        {
            return;
        }

        var taskList = events
            .Select(x => _eventBus.PublishDomainEventAsync(x, ct))
            .ToArray();

        await Task.WhenAll(taskList);
    }

    private IEnumerable<IDomainEvent> ExtractDomainEvents()
    {
        var events = _dbContext.ChangeTracker
            .Entries<IDomainEventsAccessor>()
            .Select(x => x.Entity)
            .SelectMany(aggregateRoot =>
            {
                var domainEvents = aggregateRoot.GetDomainEvents();
                // TODO: preserve from events lost!!! error handling need
                aggregateRoot.ClearDomainEvents();
                return domainEvents;
            });

        return events;
    }
}
