using Bi.Application.Abstractions;
using ErrorOr;

namespace Bi.Infrastructure.SourceLinks;

internal sealed class ClickhouseLink : ISourceLink
{
    public Task<ErrorOr<Success>> CheckConnectionStringFormat(
        string connectionString,
        CancellationToken ct) => throw new NotImplementedException();

    public Task<ErrorOr<Success>> CheckConnection(
        string connectionString,
        CancellationToken ct) => throw new NotImplementedException();

    public Task<ErrorOr<string>> GrabSchema(
        string connectionString,
        CancellationToken ct) => throw new NotImplementedException();
}
