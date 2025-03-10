using Bi.Application.Diagnostics;
using Bi.Contracts.GetDbSource;
using Bi.Domain.Abstractions;
using Bi.Domain.DataSources;
using ErrorOr;
using MediatR;

namespace Bi.Application.Handlers;

public sealed class GetDbSourceHandler : IRequestHandler<
    GetDbSourceQuery,
    ErrorOr<GetDbSourceQueryResult>>
{
    private readonly IDbSourceRepository _dbSourceRepository;

    public GetDbSourceHandler(IDbSourceRepository dataSourceRepository)
    {
        _dbSourceRepository = dataSourceRepository;
    }

    public async Task<ErrorOr<GetDbSourceQueryResult>> Handle(
        GetDbSourceQuery request,
        CancellationToken cancellationToken)
    {
        if (!DbSourceId.TryFrom(request.DbSourceId, out var id))
        {
            return ApplicationErrors.BadIdFormat;
        }

        var source = await _dbSourceRepository.Find(id, cancellationToken);
        if (source is null)
        {
            return ApplicationErrors.DbSourceNotFound;
        }

        if (!Enum.TryParse<Contracts.Enums.DbSourceSchemaMode>(
            source.SchemaMode.ToString(), ignoreCase: true, out var schemaMode))
        {
            return ApplicationErrors.EnumMappingError;
        }

        if (!Enum.TryParse<Contracts.Enums.DbSourceState>(
            source.State.ToString(), ignoreCase: true, out var state))
        {
            return ApplicationErrors.EnumMappingError;
        }
        
        return new GetDbSourceQueryResult(
            DbSourceId: source.Id.Value,
            Name: source.Name,
            PrivateNotes: source.PrivateNotes,
            Description: source.Description,
            ConnectionString: source.ConnectionString,
            SchemaMode: schemaMode,
            Schema: source.Schema,
            State: state,
            StateChanged: source.StateChanged);
    }
}
