using Conductor.Domain.Diagnostics;
using Conductor.Domain.Primitives;
using ErrorOr;
using NodaTime;

namespace Conductor.Domain.Processes;

public sealed class Process : AggregateRoot<ProcessId>
{
    private readonly List<Revision> _revisions = new();

    private Process(
        ProcessId id,
        string name,
        string displayName,
        string description,
        Instant created,
        int number) : base(id)
    {
        Name = name;
        DisplayName = displayName;
        Description = description;
        Created = created;
        Number = number;
    }

    public string Name { get; init; }

    public string DisplayName { get; private set; }

    public string Description { get; private set; }

    public Instant Created { get; init; }

    public int Number { get; init; }

    public IReadOnlyList<Revision> Revisions => _revisions
        .AsReadOnly();

    public Revision Draft => _revisions
        .Where(x => x.IsDraft)
        .First();

    public static ErrorOr<Process> Create(
        string name,
        string displayName,
        string description,
        Instant now,
        int processNumber,
        int? revisionNumber = null)
    {
        List<Error> errors = [];

        // TODO: Use ErrorOr lib combine error
        if (string.IsNullOrWhiteSpace(name))
        {
            errors.Add(DomainErrors.Process.NameCanNotBeNullOrWhitespace);
        }

        if (name.Length < 3)
        {
            errors.Add(DomainErrors.Process.NameLengthMustBeGreater2);
        }

        if (!char.IsLetter(name.First()))
        {
            errors.Add(DomainErrors.Process.NameFirstCharMustBeLetter);
        }

        if (!name.All(x => char.IsLetter(x) || char.IsDigit(x) || x == '_'))
        {
            errors.Add(DomainErrors.Process.NameAllCharsMustBeLetterOrDigitOrUnderscore);
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            errors.Add(DomainErrors.Process.DisplaynameCanNotBeNullOrWhitespace);
        }

        if (displayName.Length < 3)
        {
            errors.Add(DomainErrors.Process.DisplaynameLengthMustBeGreater2);
        }

        if (now == Instant.MinValue || now == Instant.MaxValue)
        {
            errors.Add(DomainErrors.Process.NowValueIsOutOfRange);
        }

        if (processNumber == int.MaxValue || processNumber <= 0)
        {
            errors.Add(DomainErrors.Process.ProcessNumberValueIsOutOfRange);
        }

        if (revisionNumber.HasValue)
        {
            ArgumentOutOfRangeException.ThrowIfEqual(revisionNumber.Value, int.MaxValue);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(revisionNumber.Value);
        }

        if (errors.Count > 0)
        {
            return errors.ToErrorOr<Process>();
        }

        var id = new ProcessId(Guid.NewGuid());
        
        var process = new Process(
            id,
            name,
            displayName,
            description,
            now,
            processNumber);

        var draft = Revision.CreateDraft(
            processId: id,
            now: now,
            revisionNumber: revisionNumber ?? 1,
            releaseNotes: string.Empty);

        process._revisions.Add(draft);

        return process;
    }

    public void ChangeDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName) || displayName.Length < 3)
        {
            throw new ArgumentException(nameof(displayName));
        }

        DisplayName = displayName;
    }

    public void ChangeProcessDescription(string description)
    {
        Description = description;
    }

    public void PublishDraft(
        Instant now,
        int? newRevisionNumber = null)
    {
        var oldDraft = Draft;

        if (newRevisionNumber.HasValue)
        {
            ArgumentOutOfRangeException.ThrowIfEqual(newRevisionNumber.Value, int.MaxValue);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(newRevisionNumber.Value);

            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(
                newRevisionNumber.Value, oldDraft.Number);
        }

        ArgumentOutOfRangeException.ThrowIfLessThan(now, oldDraft.Created);

        var newDraft = Revision.CreateDraft(
            processId: Id,
            now: now,
            revisionNumber: newRevisionNumber ?? oldDraft.Number + 1,
            releaseNotes: string.Empty);

        _revisions.Add(newDraft);

        oldDraft.Publish(now);
    }
}
