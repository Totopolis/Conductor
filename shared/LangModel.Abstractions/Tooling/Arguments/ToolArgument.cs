namespace LangModel.Tooling.Abstractions.Arguments;

public abstract class ToolArgument
{
    public required string Name { get; init; }

    public required string Description { get; init; }

    public required bool IsRequired { get; init; }
}
