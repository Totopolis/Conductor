using Bi.Domain.Diagnostics;
using Bi.Domain.Events;
using Domain.Shared;
using ErrorOr;
using NodaTime;

namespace Bi.Domain.Sources;

public sealed class Source : AggregateRoot<SourceId>
{
    private Source(
        SourceId id,
        SourceKind kind,
        string name,
        string userNotes,
        string description,
        string connectionString,
        string schema,
        string aiNotes,
        SourceState state,
        Instant stateChanged) : base(id)
    {
        Kind = kind;
        Name = name;
        UserNotes = userNotes;
        Description = description;
        ConnectionString = connectionString;
        Schema = schema;
        AiNotes = aiNotes;
        State = state;
        StateChanged = stateChanged;
    }

    public SourceKind Kind { get; init; }

    public string Name { get; private set; }

    // This field is not passed to the LLM and Not initiate setup.
    public string UserNotes { get; private set; }

    public string Description { get; private set; }

    // This field is not passed to the LLM.
    public string ConnectionString { get; private set; }

    public string Schema { get; private set; }

    // This filed received from LLM and user-readonly.
    public string AiNotes { get; private set; }

    public SourceState State { get; private set; }

    public Instant StateChanged { get; private set; }

    public static ErrorOr<Source> CreateNew(
        string kind,
        string name,
        string privateNotes,
        string description,
        string connectionString,
        string schema,
        Instant now)
    {
        var errors = ValidateFields(
            kind,
            name,
            privateNotes,
            description,
            connectionString,
            schema,
            now).ToList();

        if (errors.Count > 0)
        {
            return errors;
        }

        var id = SourceId.From(Guid.CreateVersion7());
        var dataSource = new Source(
            id,
            kind: SourceKind.FromName(kind, ignoreCase: true),
            name: name,
            userNotes: privateNotes,
            description: description,
            connectionString: connectionString,
            schema: schema,
            aiNotes: string.Empty,
            state: SourceState.Disabled,
            stateChanged: now);

        dataSource.ChangedAction();
        dataSource.SetState(SourceState.Setup, now);

        return dataSource;
    }

    public static IEnumerable<Error> ValidateFields(
        string kind,
        string name,
        string privateNotes,
        string description,
        string connectionString,
        string schema,
        Instant now)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
        {
            yield return DomainErrors.BadNameFormat;
        }

        // TODO: check name only latin, underscore or digits and etc...
        if (now == Instant.MinValue ||
            now == Instant.MaxValue ||
            now < Instant.FromUtc(2025, 1, 1, 0, 0))
        {
            yield return DomainErrors.BadDateTimeValue;
        }

        if (!SourceKind.TryFromName(kind, ignoreCase: true, out _))
        {
            yield return DomainErrors.UnknownSourceKind;
        }
    }

    public void SetState(SourceState newState, Instant now)
    {
        if (newState != State)
        {
            State = newState;
            StateChanged = now;
        }

        if (newState.IsSetup)
        {
            RaiseDomainEvent(new NeedSetupSource(
                Id: Id,
                Name: Name));
        }
    }

    public void UpdateOnlySchema(string schema)
    {
        if (schema != Schema)
        {
            Schema = schema;
            ChangedAction();
        }
    }

    public ErrorOr<Success> RaiseNeedUpdateEventOrError(
        string name,
        string privateNotes,
        string description,
        string connectionString,
        string schema,
        Instant now)
    {
        var errors = ValidateFields(
            kind: Kind.Name,
            name: name,
            privateNotes: privateNotes,
            description: description,
            connectionString: connectionString,
            schema: schema,
            now: now).ToList();

        if (errors.Count > 1)
        {
            return errors;
        }

        RaiseDomainEvent(new NeedUpdateSource(
            Id: Id,
            Name: name,
            PrivateNotes: privateNotes,
            Description: description,
            ConnectionString: connectionString,
            Schema: schema));

        return Result.Success;
    }

    public void UpdateDefinition(
        string name,
        string privateNotes,
        string description,
        string connectionString,
        string schema)
    {
        bool changes = false;

        if (name != Name)
        {
            Name = name;
            changes = true;
        }

        if (privateNotes != UserNotes)
        {
            UserNotes = privateNotes;
            changes = true;
        }

        if (description != Description)
        {
            Description = description;
            changes = true;
        }

        if (connectionString != ConnectionString)
        {
            ConnectionString = connectionString;
            changes = true;
        }

        if (schema != Schema)
        {
            Schema = schema;
            changes = true;
        }

        if (changes)
        {
            ChangedAction();
        }
    }

    private void ChangedAction()
    {
        var notChanges = GetDomainEvents()
            .Where(x => x is not SourceChanged)
            .ToList();

        ClearDomainEvents();

        notChanges.Add(new SourceChanged(Id: Id, Name: Name));

        // Reraise merged changes
        foreach (var it in notChanges)
        {
            RaiseDomainEvent(it);
        }
    }
}
