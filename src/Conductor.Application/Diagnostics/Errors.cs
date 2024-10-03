using ErrorOr;

namespace Conductor.Application.Diagnostics;

public static partial class ApplicationErrors
{
    public static Error CommandHandlerUnhandledException =
        Error.Failure(
            code: "20_001",
            description: "Command handler unhandled exception");
}
