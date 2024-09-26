using Conductor.Domain;
using Conductor.Domain.Deployments;
using Conductor.Domain.Processes;
using Conductor.Domain.ValueObjects;

namespace Domain.Tests.Processes;

public class CreateDeploymentsTests
{
    private readonly Process _process;
    private readonly Revision _revision;

    public CreateDeploymentsTests()
    {
        (_process, _revision) = TestData.CreateProcessWithOneRevision();
    }

    [Fact]
    public void SuccessCreate()
    {
        // Act
        var deployment = Deployment.CreateDraft(
            TimeProvider.System.GetInstantNow(),
            deploymentNumber: 1,
            notes: string.Empty);

        // Assert
        Assert.NotEqual(Guid.Empty, deployment.Id.Id);
        Assert.Equal(1, deployment.Number);
        Assert.Equal(DeploymentState.Draft, deployment.State);
        Assert.Empty(deployment.Targets);
    }

    [Theory, CombinatorialData]
    public void FailCreateByNumber(
        [CombinatorialValues(int.MinValue, -1, 0, int.MaxValue)] int number)
    {
        var createDeployment = () => Deployment.CreateDraft(
            TimeProvider.System.GetInstantNow(),
            deploymentNumber: number,
            notes: string.Empty);

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => createDeployment());

        Assert.Contains("deploymentNumber", ex.Message);
    }
}
