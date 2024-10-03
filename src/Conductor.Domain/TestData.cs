using Conductor.Domain.Deployments;
using Conductor.Domain.Processes;
using Conductor.Domain.ValueObjects;

namespace Conductor.Domain;

public static class TestData
{
    public static Func<Process> CreateProcessWithOnlyDraft = () => Process.Create(
        name: "hairstuff",
        displayName: "Hire stuff",
        description: "One of the most hr process",
        now: TimeProvider.System.GetInstantNow(),
        processNumber: 1).Value;

    public static Func<(Process, Revision)> CreateProcessWithOneRevision = () =>
    {
        var process = CreateProcessWithOnlyDraft();
        process.PublishDraft(now: TimeProvider.System.GetInstantNow());

        var revision = process.Revisions.First();
        return (process, revision);
    };

    public static Func<(Deployment, Process, Revision)> CreateDeploymentDraft = () =>
    {
        var (process, revision) = CreateProcessWithOneRevision();

        var deployment = Deployment.CreateDraft(
            now: TimeProvider.System.GetInstantNow(),
            deploymentNumber: 1,
            notes: string.Empty);

        Target[] targets = [Target.Create(process.Id, revision.Id, 1, 10)];

        deployment.ReplaceTargets(targets);

        return (deployment, process, revision);
    };
}
