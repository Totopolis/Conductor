using System.Text.Json.Serialization;

namespace Lang.Contracts.Ask;

public sealed class AskSequence
{
    [JsonConstructor]
    internal AskSequence()
    {
    }

    public required IReadOnlyList<SequenceItem> Items { get; init; }
}
