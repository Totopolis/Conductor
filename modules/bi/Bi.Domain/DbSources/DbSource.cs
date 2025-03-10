using Bi.Domain.Diagnostics;
using Bi.Domain.Events;
using Domain.Shared;
using ErrorOr;
using NodaTime;

namespace Bi.Domain.DataSources;

public sealed class DbSource : AggregateRoot<DbSourceId>
{
    private DbSource(
        DbSourceId id,
        DbSourceKind kind,
        string name,
        string privateNotes,
        string description,
        string connectionString,
        DbSourceSchemaMode schemaMode,
        string schema,
        DbSourceState state,
        Instant stateChanged) : base(id)
    {
        Kind = kind;
        Name = name;
        PrivateNotes = privateNotes;
        Description = description;
        ConnectionString = connectionString;
        SchemaMode = schemaMode;
        Schema = schema;
        State = state;
        StateChanged = stateChanged;
    }

    public DbSourceKind Kind { get; init; }

    public string Name { get; private set; }

    // This field is not passed to the LLM and Not initiate setup.
    public string PrivateNotes { get; private set; }

    public string Description { get; private set; }

    // This field is not passed to the LLM.
    public string ConnectionString { get; private set; }

    public DbSourceSchemaMode SchemaMode { get; private set; }

    public string Schema { get; private set; }

    public DbSourceState State { get; private set; }

    public Instant StateChanged { get; private set; }

    public static ErrorOr<DbSource> CreateNew(
        string kind,
        string name,
        string privateNotes,
        string description,
        string connectionString,
        string schemaMode,
        string schema,
        Instant now)
    {
        var errors = ValidateFields(
            kind,
            name,
            privateNotes,
            description,
            connectionString,
            schemaMode,
            schema,
            now).ToList();

        if (errors.Count > 0)
        {
            return errors;
        }

        var id = DbSourceId.From(Guid.CreateVersion7());
        var dataSource = new DbSource(
            id,
            kind: DbSourceKind.FromName(kind, ignoreCase: true),
            name: name,
            privateNotes: privateNotes,
            description: description,
            connectionString: connectionString,
            schemaMode: DbSourceSchemaMode.FromName(schemaMode, ignoreCase: true),
            schema: schema,
            state: DbSourceState.Setup,
            stateChanged: now);

        dataSource.ChangedAction();

        return dataSource;
    }

    public static IEnumerable<Error> ValidateFields(
        string kind,
        string name,
        string privateNotes,
        string description,
        string connectionString,
        string schemaMode,
        string schema,
        Instant now)
    {
        if (!DbSourceSchemaMode.TryFromName(schemaMode, ignoreCase: true, out _))
        {
            yield return DomainErrors.UnknownSchemaMode;
        }

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

        if (!DbSourceKind.TryFromName(kind, ignoreCase: true, out _))
        {
            yield return DomainErrors.UnknownDbSourceKind;
        }
    }

    public void SetState(DbSourceState newState, Instant now)
    {
        if (newState != State)
        {
            State = newState;
            StateChanged = now;
        }

        if (newState.IsSetup)
        {
            RaiseDomainEvent(new NeedSetupDbSource(
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
        string schemaMode,
        string schema,
        Instant now)
    {
        var errors = ValidateFields(
            kind: Kind.Name,
            name: name,
            privateNotes: privateNotes,
            description: description,
            connectionString: connectionString,
            schemaMode: schemaMode,
            schema: schema,
            now: now).ToList();

        if (errors.Count > 1)
        {
            return errors;
        }

        RaiseDomainEvent(new NeedUpdateDbSource(
            Id: Id,
            Name: name,
            PrivateNotes: privateNotes,
            Description: description,
            ConnectionString: connectionString,
            SchemaMode: DbSourceSchemaMode.FromName(schemaMode, ignoreCase: true),
            ManualSchema: schema));

        return Result.Success;
    }

    public void UpdateDefinition(
        string name,
        string privateNotes,
        string description,
        string connectionString,
        DbSourceSchemaMode schemaMode,
        string schema)
    {
        bool changes = false;

        if (name != Name)
        {
            Name = name;
            changes = true;
        }

        if (privateNotes != PrivateNotes)
        {
            PrivateNotes = privateNotes;
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

        if (schemaMode != SchemaMode)
        {
            SchemaMode = schemaMode;
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
            .Where(x => x is not DbSourceChanged)
            .ToList();

        ClearDomainEvents();

        notChanges.Add(new DbSourceChanged(Id: Id, Name: Name));

        // Reraise merged changes
        foreach (var it in notChanges)
        {
            RaiseDomainEvent(it);
        }
    }
}
