using Infrastructure.Tests.Sample;
using Testcontainers.PostgreSql;

namespace Infrastructure.Tests;

public class SampleTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("retranslator")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private const int TestTimeoutMs = 30_000;
    private readonly CancellationTokenSource _timeoutTokenSource = new(TestTimeoutMs);
    private readonly CancellationTokenSource _resetTokenSource = new();

    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    [Fact]
    public async Task Test1()
    {
        var cs = _dbContainer.GetConnectionString();
        var db = new SampleDbContext(cs);

        await db.EnsureDatabaseStructureCreated(_timeoutTokenSource.Token);

        var foo = FooEntity.Create(
            1,
            "2",
            BooValue.Create(33, "fin"));

        db.Set<FooEntity>().Add(foo);
        await db.SaveChangesAsync(_timeoutTokenSource.Token);
    }
}
