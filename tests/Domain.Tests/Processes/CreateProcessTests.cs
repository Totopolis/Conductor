using Conductor.Domain;
using Conductor.Domain.Diagnostics;
using Conductor.Domain.Processes;
using ErrorOr;
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
            processNumber: 1).Value;

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
        var result = Process.Create(
            name: name,
            displayName: ProcessDisplayName,
            description: ProcessDescription,
            now: _now,
            processNumber: 1);

        List<Error> expectedErrors = [
            DomainErrors.Process.NameCanNotBeNullOrWhitespace,
            DomainErrors.Process.NameFirstCharMustBeLetter,
            DomainErrors.Process.NameLengthMustBeGreater2,
            DomainErrors.Process.NameAllCharsMustBeLetterOrDigitOrUnderscore];

        Assert.True(result.IsError);
        Assert.Contains(expectedErrors, x => x == result.FirstError);
    }

    [Theory, CombinatorialData]
    public void FailCreateByDisplayName(
        [CombinatorialValues("", "dn", null)] string displayName)
    {
        var result = Process.Create(
            name: ProcessName,
            displayName: displayName,
            description: ProcessDescription,
            now: _now,
            processNumber: 1);

        List<Error> expectedErrors = [
            DomainErrors.Process.DisplaynameCanNotBeNullOrWhitespace,
            DomainErrors.Process.DisplaynameLengthMustBeGreater2];

        Assert.True(result.IsError);
        Assert.Contains(expectedErrors, x => x == result.FirstError);
    }

    [Theory, CombinatorialData]
    public void FailCreateByNumber(
        [CombinatorialValues(int.MinValue, -1, 0, int.MaxValue)] int number)
    {
        var result = Process.Create(
            name: ProcessName,
            displayName: ProcessDisplayName,
            description: ProcessDescription,
            now: _now,
            processNumber: number);

        Assert.True(result.IsError);
        Assert.Equal(DomainErrors.Process.ProcessNumberValueIsOutOfRange, result.FirstError);
    }
}
