namespace LangModel.Tooling.Abstractions.Arguments;

public abstract class ToolArgument
{
    public string Name { get; protected set; } = default!;

    public string Description { get; protected set; } = default!;

    public bool IsRequired { get; protected set; }
}
