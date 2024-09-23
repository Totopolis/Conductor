namespace LangModel.Tooling.Abstractions.Arguments;

public sealed class StringArgument : ToolArgument
{
    public StringArgument(
        string name,
        string description,
        bool isRequired = true)
    {
        Name = name;
        Description = description;
        IsRequired = isRequired;
    }
}
