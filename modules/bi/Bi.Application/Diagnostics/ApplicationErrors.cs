using ErrorOr;

namespace Bi.Application.Diagnostics;

public static class ApplicationErrors
{
    public static readonly Error UnexpectedError = Error.Unexpected(
        code: "Bi.Application.UnexpectedError",
        description: "Unexpected error, see logs");

    public static readonly Error SourceNotFound = Error.NotFound(
        code: "Bi.Application.DataSourceNotFound",
        description: "DataSourceId not found");

    public static readonly Error BadIdFormat = Error.Validation(
        code: "Bi.Application.BadIdFormat",
        description: "Bad Id format");

    public static readonly Error EnumMappingError = Error.Unexpected(
        code: "Bi.Application.EnumMappingError",
        description: "Enum mapping error");

    public static readonly Error BadConnectionStringFormat = Error.Failure(
       code: "Bi.Application.BadConnectionStringFormat",
       description: "Bad сonnection string format");

    public static readonly Error SourceConnectionFailed = Error.Failure(
       code: "Bi.Application.SourceConnectionFailed",
       description: "Source connection failed");
}
