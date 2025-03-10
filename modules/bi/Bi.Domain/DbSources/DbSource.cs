using Bi.Domain.Diagnostics;
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
        DbSourceSchemaMode schemaMode,
        string schema,
        Instant now)
    {
        var errors = ValidateCreateFields(
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
            schemaMode: schemaMode,
            schema: schema,
            state: DbSourceState.Setup,
            stateChanged: now);

        return dataSource;
    }

    public static IEnumerable<Error> ValidateCreateFields(
        string kind,
        string name,
        string privateNotes,
        string description,
        string connectionString,
        DbSourceSchemaMode schemaMode,
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

        if (!DbSourceKind.TryFromName(kind, ignoreCase: true, out _))
        {
            yield return DomainErrors.UnknownDbSourceKind;
        }
    }

    public void ChangeDescription(string newDescription)
    {
        Description = newDescription;
    }
}
