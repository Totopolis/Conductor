using ErrorOr;
using LangModel.Abstractions.Errors;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace LangModel.Tooling.Abstractions;

public sealed class ToolResponse
{
    private readonly JsonDocument _doc;

    private ToolResponse(JsonDocument doc)
    {
        _doc = doc;
    }

    public JsonElement Data => _doc.RootElement;

    public string Content
    {
        get
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                WriteIndented = true
            };

            var content = JsonSerializer.Serialize(_doc, options);
            return content;
        }
    }

    public static ErrorOr<ToolResponse> CreateFrom<T>(T responseData) where T : class
    {
        try
        {
            var doc = JsonSerializer.SerializeToDocument<T>(responseData);
            if (doc is null)
            {
                return LangModelErrors.ToolRequest.CastToDataError;
            }

            return new ToolResponse(doc);
        }
        catch
        {
            return LangModelErrors.ToolRequest.CastToDataError;
        }
    }
}
