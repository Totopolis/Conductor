namespace Lang.Contracts.Ask;

public sealed class SequenceBuilder
{
    private readonly List<SequenceItem> _items = new();

    private SequenceBuilder()
    {
    }

    public static SequenceBuilder Create()
    {
        return new SequenceBuilder();
    }

    // TODO: check sequence consistency
    public SequenceBuilder AddSystemPrompt(string content)
    {
        _items.Add(new SequenceItem(
            Role: "system",
            Content: content));

        return this;
    }

    // TODO: check sequence consistency
    public SequenceBuilder AddUserItem(string content)
    {
        _items.Add(new SequenceItem(
            Role: "user",
            Content: content));

        return this;
    }

    public SequenceBuilder AddAssistantItem(string content)
    {
        _items.Add(new SequenceItem(
            Role: "assistant",
            Content: content));

        return this;
    }

    public AskSequence Build()
    {
        return new AskSequence
        {
            Items = _items
        };
    }
}
