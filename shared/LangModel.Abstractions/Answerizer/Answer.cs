using LangModel.Abstractions.Common;

namespace LangModel.Abstractions.Answerizer;

public abstract class Answer
{
    public required UsageValue Usage { get; init; }

    public abstract void FillSequence(ISequenceFiller filler);
}
