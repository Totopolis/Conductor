using Bi.Contracts.CreateDbSource;
using Bi.Domain.Abstractions;
using ErrorOr;
using MediatR;

namespace Bi.Application.Handlers;

public sealed class GetSourcesHandler : IRequestHandler<
    GetSourcesQuery,
    ErrorOr<GetSourcesQueryResult>>
{
    private readonly IDbSourceRepository _dbSourceRepository;

    public GetSourcesHandler(IDbSourceRepository dataSourceRepository)
    {
        _dbSourceRepository = dataSourceRepository;
    }

    public async Task<ErrorOr<GetSourcesQueryResult>> Handle(
        GetSourcesQuery request,
        CancellationToken cancellationToken)
    {
        var dbSources = await _dbSourceRepository.GetAll(cancellationToken);

        var sources = dbSources
            .Select(x => new GetSourcesQueryResult.Source(
                Id: x.Id.Value,
                kind: x.Kind.Name,
                Name: x.Name,
                State: x.State.Name,
                StateChanged: x.StateChanged))
            .ToList()
            .AsReadOnly();

        return new GetSourcesQueryResult(Sources: sources);
    }
}
