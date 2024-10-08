using Conductor.Infrastructure;
using Conductor.Infrastructure.Database;
using Conductor.Infrastructure.Settings;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NSubstitute;
using Testcontainers.PostgreSql;

namespace Infrastructure.Tests;

public abstract class AbstractRepositoryTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("conductor")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private const int TestTimeoutMs = 60_000;
    protected readonly CancellationTokenSource _timeoutTokenSource = new(TestTimeoutMs);
    protected readonly CancellationTokenSource _resetTokenSource = new();

    protected IHost _host = default!;

    public async Task DisposeAsync()
    {
        _host.Dispose();
        await _dbContainer.StopAsync();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        _host = await StartApp();
    }

    private async Task<IHost> StartApp()
    {
        var builder = Host.CreateEmptyApplicationBuilder(null);

        var pgConnectionString = _dbContainer.GetConnectionString();
        var infrastructureSettings = Options.Create(new InfrastructureSettings
        {
            DatabaseConnectionString = pgConnectionString
        });

        builder.Services.AddSingleton(infrastructureSettings);

        builder.Services
            .AddInfrastructureServices(builder.Configuration);
        // .AddMasstransitLocal(builder.Configuration);

        builder.Services.AddSingleton(Substitute.For<IPublishEndpoint>());

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ConductorDbContext>();
            await dbContext.Database.EnsureCreatedAsync(_timeoutTokenSource.Token);
        }

        // TODO: apply awaiter?
        var awaiter = app.StartAsync(_timeoutTokenSource.Token);

        return app;
    }
}
