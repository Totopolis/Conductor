using CSharpFunctionalExtensions;
using NodaTime;
using System.Text.Json;

namespace Conductor.Domain.Processes;

public sealed class Revision : Entity<RevisionId>
{
    private Revision(
        RevisionId id,
        ProcessId processId,
        Instant created,
        int number,
        bool isDraft,
        string releaseNotes,
        JsonElement content,
        Instant published) : base(id)
    {
        ProcessId = processId;
        Created = created;
        Number = number;
        IsDraft = isDraft;
        ReleaseNotes = releaseNotes;
        Content = content;
        Published = published;
    }

    public ProcessId ProcessId { get; init; }

    public Instant Created { get; private set; }

    public int Number { get; init; }

    // TODO: use enum Kind - Draft, Revision, MarkToDelete
    public bool IsDraft { get; private set; }

    public string ReleaseNotes { get; private set; }

    public JsonElement Content { get; private set; }

    public Instant Published { get; private set; }

    internal static Revision CreateDraft(
        ProcessId processId,
        Instant now,
        int revisionNumber,
        string releaseNotes)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(processId.Id, Guid.Empty);

        ArgumentOutOfRangeException.ThrowIfEqual(now, Instant.MinValue);
        ArgumentOutOfRangeException.ThrowIfEqual(now, Instant.MaxValue);

        ArgumentOutOfRangeException.ThrowIfEqual(revisionNumber, int.MaxValue);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(revisionNumber);
        
        var id = new RevisionId(Guid.NewGuid());
        return new Revision(
            id,
            processId,
            created: now,
            number: revisionNumber,
            isDraft: true,
            releaseNotes,
            content: Helpers.EmptyJsonElement,
            published: Instant.MinValue);
    }

    public void ChangeReleaseNotes(string releaseNotes)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(IsDraft, false);

        ReleaseNotes = releaseNotes;
    }

    public void Publish(Instant now)
    {
        Published = now;
        IsDraft = false;
    }

    public void EditDraft(JsonElement content)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(IsDraft, false);

        Content = content;
    }
}
