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

    public static readonly Error BadIdFormat = Error.Validation(
        code: "Bi.Application.BadIdFormat",
        description: "Bad Id format");

    public static readonly Error DbSourceBusy = Error.Failure(
        code: "Bi.Application.DbSourceBusy",
        description: "DbSource busy now");
}
