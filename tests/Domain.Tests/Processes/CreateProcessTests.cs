using Conductor.Domain;
using Conductor.Domain.Processes;
using NodaTime;

namespace Domain.Tests.Processes;

public class CreateProcessTests
{
    private const string ProcessName = "sample";
    private const string ProcessDisplayName = "some";
    private const string ProcessDescription = "long text";

    private readonly Instant _now = TimeProvider.System.GetInstantNow();

    [Fact]
    public void SuccessCreate()
    {
        var process = Process.Create(
            name: ProcessName,
            displayName: ProcessDisplayName,
            description: ProcessDescription,
            now: _now,
            processNumber: 1);

        Assert.Equal(ProcessName, process.Name);
        Assert.Equal(ProcessDisplayName, process.DisplayName);
        Assert.Equal(ProcessDescription, process.Description);
        Assert.NotEqual(Guid.Empty, process.Id.Id);
    }

    [Theory, CombinatorialData]
    public void FailCreateByName([CombinatorialValues(
        "",
        " ",
        "nm",
        null,
        " name",
        "nam e",
        "3name",
        "_name")] string name)
    {
        var createProcess = () => Process.Create(
            name: name,
            displayName: ProcessDisplayName,
            description: ProcessDescription,
            now: _now,
            processNumber: 1);

        var ex = Assert.Throws<ArgumentException>(() => createProcess());

        Assert.Contains("name", ex.Message);
    }

    [Theory, CombinatorialData]
    public void FailCreateByDisplayName(
        [CombinatorialValues("", "dn", null)] string displayName)
    {
        var createProcess = () => Process.Create(
            name: ProcessName,
            displayName: displayName,
            description: ProcessDescription,
            now: _now,
            processNumber: 1);

        var ex = Assert.Throws<ArgumentException>(() => createProcess());

        Assert.Contains("displayName", ex.Message);
    }

    [Theory, CombinatorialData]
    public void FailCreateByNumber(
        [CombinatorialValues(int.MinValue, -1, 0, int.MaxValue)] int number)
    {
        var createProcess = () => Process.Create(
            name: ProcessName,
            displayName: ProcessDisplayName,
            description: ProcessDescription,
            now: _now,
            processNumber: number);

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => createProcess());

        Assert.Contains("processNumber", ex.Message);
    }
}