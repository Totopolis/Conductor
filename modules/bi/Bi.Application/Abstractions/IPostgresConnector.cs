using ErrorOr;

namespace Bi.Application.Abstractions;

public interface IPostgresConnector
{
    Task<ErrorOr<Success>> CheckConnection(string connectionString, CancellationToken ct);

    Task<ErrorOr<string>> GrabSchema(string connectionString, CancellationToken ct);
}
