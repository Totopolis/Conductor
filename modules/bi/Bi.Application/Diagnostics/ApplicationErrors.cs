using ErrorOr;

namespace Bi.Application.Diagnostics;

public static class ApplicationErrors
{
    public static readonly Error DataSourceNotFound = Error.NotFound(
        code: "Bi.Application.DataSourceNotFound",
        description: "DataSourceId not found");
}
