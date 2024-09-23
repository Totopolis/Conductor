using Conductor.Domain;
using Conductor.Domain.Deployments;
using Conductor.Domain.Processes;
using Conductor.Domain.ValueObjects;

namespace Domain.Tests.Processes;

public class ReplaceTargetsTests
{
    private readonly Deployment _deployment;
    private readonly Process _process;
    private readonly Revision _revision;

    private readonly Target _target;

    public ReplaceTargetsTests()
    {
        (_deployment, _process, _revision) = TestData.CreateDeploymentDraft();
        _target = Target.Create(_process.Id, _revision.Id, 1, 10);
    }

    [Fact]
    public void CreateTargets()
    {
        Target[] _targets = [_target];
        _deployment.ReplaceTargets(_targets);

        Assert.Single(_deployment.Targets);
    }

    [Fact]
    public void FailedCreateTargets()
    {
        var tryCreate = () =>
        {
            Target[] _targets = [_target, _target];
            _deployment.ReplaceTargets(_targets);
        };

        Assert.Throws<ArgumentException>(() => tryCreate());
    }
}
