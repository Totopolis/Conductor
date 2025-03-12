using Bi.Application.Abstractions;
using Bi.Application.Diagnostics;
using ErrorOr;
using Npgsql;

namespace Bi.Infrastructure;

internal sealed class PostgresConnector : IPostgresConnector
{
    public async Task<ErrorOr<Success>> CheckConnectionStringFormat(
        string connectionString,
        CancellationToken ct)
    {
        await Task.CompletedTask;

        try
        {
            _ = new NpgsqlConnectionStringBuilder(connectionString);
        }
        catch
        {
            return ApplicationErrors.BadConnectionStringFormat;
        }

        return Result.Success;
    }

    public async Task<ErrorOr<Success>> CheckConnection(
        string connectionString,
        CancellationToken ct)
    {
        var errorOrSuccess = await CheckConnectionStringFormat(connectionString, ct);
        if (errorOrSuccess.IsError)
        {
            return errorOrSuccess;
        }

        try
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(ct);
        }
        catch (NpgsqlException)
        {
            return ApplicationErrors.SourceConnectionFailed;
        }
        catch (Exception)
        {
            return ApplicationErrors.SourceConnectionFailed;
        }

        return Result.Success;
    }

    public async Task<ErrorOr<string>> GrabSchema(
        string connectionString,
        CancellationToken ct)
    {
        var errorOrSuccess = await CheckConnection(connectionString, ct);
        if (errorOrSuccess.IsError)
        {
            return errorOrSuccess.Errors;
        }

        try
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(ct);


        }
        catch (NpgsqlException)
        {
            return ApplicationErrors.SourceConnectionFailed;
        }
        catch (Exception)
        {
            return ApplicationErrors.SourceConnectionFailed;
        }

        return "<NO SCHEMA>";
    }
}
