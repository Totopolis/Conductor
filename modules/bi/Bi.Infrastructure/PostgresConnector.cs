using Bi.Application.Abstractions;
using ErrorOr;

namespace Bi.Infrastructure;

internal sealed class PostgresConnector : IPostgresConnector
{
    public async Task<ErrorOr<Success>> CheckConnection(
        string connectionString,
        CancellationToken ct)
    {
        await Task.Delay(100);
        return Result.Success;
    }

    public async Task<ErrorOr<string>> GrabSchema(
        string connectionString,
        CancellationToken ct)
    {
        await Task.Delay(100);
        return "<NO SCHEMA>";
    }
}
