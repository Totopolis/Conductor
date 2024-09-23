using System.Text.Json;

namespace LangModel.Tooling.Abstractions;

public abstract class ToolRequest
{
    public abstract JsonElement Data { get; }
}
