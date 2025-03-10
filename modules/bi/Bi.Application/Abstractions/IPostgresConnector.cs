using ErrorOr;
using MediatR;

namespace Bi.Application.Abstractions;

public interface IPostgresConnector
{
    Task<ErrorOr<Unit>> CheckConnection(string connectionString, CancellationToken ct);

    Task<ErrorOr<string>> GrabSchema(string connectionString, CancellationToken ct);
}
