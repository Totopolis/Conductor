using Conductor.Domain.Abstractions;
using Conductor.Domain.Numbers;
using Conductor.Domain.Primitives;
using Conductor.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Conductor.Infrastructure.Misc;

internal class NumberService : INumberService
{
    private readonly ConductorDbContext _db;

    public NumberService(ConductorDbContext db)
    {
        _db = db;
    }

    public async Task<int> GenerateGeneral<T, TID>(CancellationToken ct)
        where T : AggregateRoot<TID>
        where TID : struct, IComparable<TID>
    {
        var kindName = nameof(GeneratorKind.General);
        var kind = GeneratorKind.FromName(kindName);

        return await Generate(kind, ct);
    }

    public async Task<int> GenerateSeparated<T, TID>(CancellationToken ct)
        where T : AggregateRoot<TID>
        where TID : struct, IComparable<TID>
    {
        var kindName = typeof(T).Name;
        var kind = GeneratorKind.FromName(kindName);

        return await Generate(kind, ct);
    }

    private async Task<int> Generate(GeneratorKind kind, CancellationToken ct)
    {
        // TODO: need retry logic
        using var tran = await _db.Database.BeginTransactionAsync(
            isolationLevel: System.Data.IsolationLevel.Serializable,
            cancellationToken: ct);

        try
        {
            var all = await _db.Set<Number>().ToListAsync(ct);

            var maxValue = all.Max(x => x.Value);
            var numberGenerator = all.First(x => x.Kind == kind);

            var newValue = numberGenerator.Value < maxValue ?
                maxValue + 1 :
                numberGenerator.Value + 1;

            numberGenerator.SetupNewValue(newValue);

            await _db.SaveChangesAsync(ct);
            await _db.Database.CommitTransactionAsync(ct);

            return numberGenerator.Value;
        }
        catch
        {
            await tran.RollbackAsync(ct);
            throw;
        }
    }
}
