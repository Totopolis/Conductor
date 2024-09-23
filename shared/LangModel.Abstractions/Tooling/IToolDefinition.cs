using LangModel.Tooling.Abstractions.Arguments;

namespace LangModel.Tooling.Abstractions;

public interface IToolDefinition
{
    string Name { get; }

    string Description { get; }

    IReadOnlyList<ToolArgument> Arguments { get; }

    IReadOnlyList<string> Tags { get; }
}
