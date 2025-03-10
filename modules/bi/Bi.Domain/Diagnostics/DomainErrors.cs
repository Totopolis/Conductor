using ErrorOr;

namespace Bi.Domain.Diagnostics;

public static class DomainErrors
{
    public static readonly Error BadNameFormat = Error.Validation(
        code: "Bi.Domain.BadNameFormat",
        description: "Bad name format");

    public static readonly Error BadDateTimeValue = Error.Validation(
        code: "Bi.Domain.BadDateTimeValue",
        description: "Bad DateTime value");

    public static readonly Error UnknownDbSourceKind = Error.Validation(
        code: "Bi.Domain.UnknownDbSourceKind",
        description: "Unknown db source kind");

    public static readonly Error UnexpectedError = Error.Unexpected(
        code: "Bi.Domain.UnexpectedError",
        description: "Unexpected error, see logs");

    public static readonly Error UnknownSchemaMode = Error.Unexpected(
        code: "Bi.Domain.UnknownSchemaMode",
        description: "Unknown schema mode");
}
