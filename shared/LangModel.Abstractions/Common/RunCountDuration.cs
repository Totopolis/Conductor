namespace LangModel.Abstractions.Common;

public class RunCountDuration
{
    private RunCountDuration()
    {
    }

    public required int Count { get; init; }

    public required TimeSpan Duration { get; init; }

    public static readonly RunCountDuration Empty = new RunCountDuration
    {
        Count = 0,
        Duration = TimeSpan.Zero
    };

    public static RunCountDuration Create(int count, TimeSpan duration)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(count, 0);

        return new RunCountDuration
        {
            Count = count,
            Duration = duration
        };
    }

    public static RunCountDuration operator +(RunCountDuration left, RunCountDuration right)
    {
        return new RunCountDuration
        {
            Count = left.Count + right.Count,
            Duration = left.Duration + right.Duration
        };
    }
}
