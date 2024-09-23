namespace LangModel.Tooling.Abstractions.Arguments;

public sealed class IntegerArgument : ToolArgument
{
    public IntegerArgument(
        string name,
        string description,
        bool isRequired = true)
    {
        Name = name;
        Description = description;
        IsRequired = isRequired;
    }
}