using Conductor.Domain;
using Conductor.Domain.Abstractions;
using Conductor.Domain.Processes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Tests;

public class ProcessRepositoryTests : AbstractRepositoryTests
{
    private readonly Process _process;

    public ProcessRepositoryTests()
    {
        _process = TestData.CreateProcessWithOnlyDraft();
    }

    [Fact]
    public async Task SuccessInsertProcessWithOnlyDraft()
    {
        var (repo, uow) = ResolveDbServices(_host);
        
        repo.Add(_process);
        await uow.SaveChanges();
    }

    [Fact]
    public async Task SuccessInsertProcessWithOneRevision()
    {
        var now = TimeProvider.System.GetInstantNow();
        _process.PublishDraft(now);

        var (repo, uow) = ResolveDbServices(_host);

        repo.Add(_process);
        await uow.SaveChanges();
    }

    [Fact]
    public async Task SuccessReadAggregate()
    {
        var now = TimeProvider.System.GetInstantNow();
        _process.PublishDraft(now);

        using (var scope1 = _host.Services.CreateScope())
        {
            var (repo, uow) = ResolveDbServices(_host);
            repo.Add(_process);
            await uow.SaveChanges();
        }

        using (var scope2 = _host.Services.CreateScope())
        {
            var (repo, uow) = ResolveDbServices(_host);
            var processAggregate = await repo.GetByIdWithAllRevisions(_process.Id);
            
            Assert.NotNull(processAggregate);
            Assert.Equal(_process.Id, processAggregate.Id);
            Assert.Equal(2, processAggregate.Revisions.Count);
        }
    }

    private (IProcessRepository, IUnitOfWork) ResolveDbServices(IHost host)
    {
        return (
            host.Services.GetRequiredService<IProcessRepository>(),
            host.Services.GetRequiredService<IUnitOfWork>());
    }
}
