namespace Conductor.Domain.Abstractions;

public interface IUnitOfWork
{
    Task SaveChanges(CancellationToken ct = default);
}
