namespace LangModel.Abstractions.Answerizer;

public abstract class Answer
{
    public required AnswerStat Stat { get; init; }

    public abstract void FillSequence(ISequenceFiller filler);
}
