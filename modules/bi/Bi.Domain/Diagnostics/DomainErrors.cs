using ErrorOr;

namespace Bi.Domain.Diagnostics;

public static class DomainErrors
{
    public static readonly Error BadNameFormat = Error.Validation(
        code: "Bi.Domain.BadNameFormat",
        description: "Bad name format");
}
