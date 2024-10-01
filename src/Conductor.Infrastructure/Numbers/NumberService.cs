using Conductor.Domain.Abstractions;

namespace Conductor.Infrastructure.Numbers;

internal class NumberService : INumberService
{
    private static int _counter = 0;

    public Task<int> GenerateNext(INumberService.GeneratorType generatorType)
    {
        // TODO: use pgsql with full isolation level on table
        _counter++;
        return Task.FromResult(_counter);
    }
}
