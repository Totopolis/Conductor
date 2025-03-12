using Bi.Contracts.CreateSource;
using Bi.Domain.Abstractions;
using ErrorOr;
using MediatR;

namespace Bi.Application.Handlers;

public sealed class GetSourcesHandler : IRequestHandler<
    GetSourcesQuery,
    ErrorOr<GetSourcesQueryResult>>
{
    private readonly ISourceRepository _sourceRepository;

    public GetSourcesHandler(ISourceRepository dataSourceRepository)
    {
        _sourceRepository = dataSourceRepository;
    }

    public async Task<ErrorOr<GetSourcesQueryResult>> Handle(
        GetSourcesQuery request,
        CancellationToken cancellationToken)
    {
        var allSources = await _sourceRepository.GetAll(cancellationToken);

        var sources = allSources
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
