using ErrorOr;
using LangModel.Abstractions.Errors;
using System.Text;
using System.Text.Json;

namespace LangModel.Tooling.Abstractions;

public sealed class ToolRequest
{
    private readonly Guid _correlationId;
    private readonly JsonDocument _doc;

    private ToolRequest(
        Guid correlationId,
        JsonDocument doc)
    {
        _correlationId = correlationId;
        _doc = doc;
    }

    public Guid CorrelationId => _correlationId;

    public JsonElement Data => _doc.RootElement;

    public static ErrorOr<ToolRequest> Create(Guid correlationId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return LangModelErrors.ToolRequest.ContentIsNullOrWhiteSpace;
        }

        // TODO: use stackalloc
        var bytes = Encoding.UTF8.GetBytes(content);
        var reader = new Utf8JsonReader(bytes);

        if (!JsonDocument.TryParseValue(ref reader, out var doc) ||
            doc is null)
        {
            return LangModelErrors.ToolRequest.ContentIsNotCorrectJson;
        }

        return new ToolRequest(correlationId, doc);
    }

    public ErrorOr<T> CastTo<T>() where T : class
    {
        try
        {
            var result = JsonSerializer.Deserialize<T>(_doc);
            if (result is null)
            {
                return LangModelErrors.ToolRequest.CastToDataError;
            }

            return result!;
        }
        catch
        {
            return LangModelErrors.ToolRequest.CastToDataError;
        }
    }
}
