namespace LangModel.Abstractions;

public abstract class Question
{
    public abstract decimal Cost { get; }

    public abstract bool NoSamplesAndNoFish { get; }

    public abstract ushort FishBlocksCount { get; }

    public abstract Question TrimTopBlockAndCreateNewQuestion();

    public abstract void FillFishSequence(ISequenceFiller fishFiller);
}
