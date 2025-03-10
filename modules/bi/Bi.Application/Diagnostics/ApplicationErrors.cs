using ErrorOr;

namespace Bi.Application.Diagnostics;

public static class ApplicationErrors
{
    public static readonly Error UnexpectedError = Error.Unexpected(
        code: "Bi.Application.UnexpectedError",
        description: "Unexpected error, see logs");

    public static readonly Error DataSourceNotFound = Error.NotFound(
        code: "Bi.Application.DataSourceNotFound",
        description: "DataSourceId not found");
}
