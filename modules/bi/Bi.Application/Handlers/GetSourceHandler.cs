using Bi.Application.Diagnostics;
using Bi.Contracts.GetSource;
using Bi.Domain.Abstractions;
using Bi.Domain.Sources;
using ErrorOr;
using MediatR;

namespace Bi.Application.Handlers;

public sealed class GetSourceHandler : IRequestHandler<
    GetSourceQuery,
    ErrorOr<GetSourceQueryResult>>
{
    private readonly ISourceRepository _sourceRepository;

    public GetSourceHandler(ISourceRepository sourceRepository)
    {
        _sourceRepository = sourceRepository;
    }

    public async Task<ErrorOr<GetSourceQueryResult>> Handle(
        GetSourceQuery request,
        CancellationToken cancellationToken)
    {
        if (!SourceId.TryFrom(request.SourceId, out var id))
        {
            return ApplicationErrors.BadIdFormat;
        }

        var source = await _sourceRepository.Find(id, cancellationToken);
        if (source is null)
        {
            return ApplicationErrors.SourceNotFound;
        }

        if (!Enum.TryParse<Contracts.Enums.SourceState>(
            source.State.ToString(), ignoreCase: true, out var state))
        {
            return ApplicationErrors.EnumMappingError;
        }
        
        return new GetSourceQueryResult(
            SourceId: source.Id.Value,
            Name: source.Name,
            UserNotes: source.UserNotes,
            Description: source.Description,
            ConnectionString: source.ConnectionString,
            Schema: source.Schema,
            AiNotes: source.AiNotes,
            State: state,
            StateChanged: source.StateChanged);
    }
}
