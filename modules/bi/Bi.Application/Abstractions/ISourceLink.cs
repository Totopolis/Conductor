using ErrorOr;

namespace Bi.Application.Abstractions;

public interface ISourceLink
{
    Task<ErrorOr<Success>> CheckConnectionStringFormat(string connectionString, CancellationToken ct);

    Task<ErrorOr<Success>> CheckConnection(string connectionString, CancellationToken ct);

    Task<ErrorOr<string>> GrabSchema(string connectionString, CancellationToken ct);
}
