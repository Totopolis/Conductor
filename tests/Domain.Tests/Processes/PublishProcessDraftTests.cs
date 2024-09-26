using Conductor.Domain;
using Conductor.Domain.Processes;

namespace Domain.Tests.Processes;

public class PublishProcessDraftTests
{
    private readonly Process _process = TestData.CreateProcessWithOnlyDraft();

    [Fact]
    public void SingleDraft()
    {
        Assert.Single(_process.Revisions, x=>x.IsDraft);
        Assert.True(_process.Draft.IsDraft);
    }

    [Fact]
    public void PublishDraft()
    {
        var now = TimeProvider.System.GetInstantNow();

        // Act
        _process.PublishDraft(now);

        // Assert
        Assert.Single(_process.Revisions, x => !x.IsDraft);

        Assert.Equal(1, _process.Revisions.First().Number);
        Assert.False(_process.Revisions.First().IsDraft);

        Assert.Equal(2, _process.Draft.Number);
        Assert.True(_process.Draft.IsDraft);
    }
}
