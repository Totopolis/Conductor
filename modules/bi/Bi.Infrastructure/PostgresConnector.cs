using Bi.Application.Abstractions;
using ErrorOr;

namespace Bi.Infrastructure;

internal sealed class PostgresConnector : IPostgresConnector
{
    public Task<ErrorOr<Success>> CheckConnection(string connectionString, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<string>> GrabSchema(string connectionString, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
