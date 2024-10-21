using LangModel.Abstractions.Common;
using System.Text.Json;

namespace LangModel.Abstractions.Diagnostics;

public interface ILangModelTracer
{
    Task Trace(
        LangModelTracerKind kind,
        JsonElement request,
        JsonElement response,
        UsageValue usage);
}
