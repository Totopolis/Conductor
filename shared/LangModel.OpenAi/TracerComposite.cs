using LangModel.Abstractions.Common;
using LangModel.Abstractions.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LangModel.OpenAi;

internal sealed class TracerComposite
{
    private readonly ILogger<TracerComposite> _logger;
    private readonly IReadOnlyList<ILangModelTracer> _tracers;

    public TracerComposite(
        ILogger<TracerComposite> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        var tracers = serviceProvider.GetService<IEnumerable<ILangModelTracer>>();
        _tracers = tracers?.ToList() ?? [];
    }

    public async Task Trace(
        LangModelTracerKind kind,
        JsonElement request,
        JsonElement response,
        UsageValue usage)
    {
        foreach (var it in _tracers)
        {
            try
            {
                await it.Trace(kind, request, response, usage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at trace langmodel");
            }
        }
    }
}
