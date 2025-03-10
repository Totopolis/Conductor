using Application.Shared.Abstractions;
using Bi.Domain.Abstractions;
using Bi.Infrastructure.Database;
using Domain.Shared;

namespace Bi.Infrastructure.Repositories;

internal class UnitOfWork : IUnitOfWork
{
    private readonly BiDbContext _dbContext;
    private readonly IEventBusPublisher _eventBus;

    public UnitOfWork(
        BiDbContext context,
        IEventBusPublisher eventBus)
    {
        _dbContext = context;
        _eventBus = eventBus;
    }

    public async Task SaveChanges(CancellationToken ct)
    {
        // var count =
        await _dbContext.SaveChangesAsync(ct);

        await PublishDomainEvents(ct);
    }

    // TODO: ConvertDomainEventsToOutboxMessages
    private async Task PublishDomainEvents(CancellationToken ct)
    {
        var events = ExtractDomainEvents().ToList();

        if (events.Count == 0)
        {
            return;
        }

        var taskList = events
            .Select(x => _eventBus.PublishDomainEvent(x, ct))
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
                // DANGER: events lost
                aggregateRoot.ClearDomainEvents();
                return domainEvents;
            });

        return events;
    }
}
