namespace Bi.Domain.Abstractions;

public interface IUnitOfWork
{
    Task SaveChanges(CancellationToken ct);
}
