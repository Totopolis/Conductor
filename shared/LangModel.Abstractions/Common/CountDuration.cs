namespace LangModel.Abstractions.Common;

public class CountDuration
{
    private CountDuration()
    {
    }

    public required int Count { get; init; }

    public required TimeSpan Duration { get; init; }

    public static readonly CountDuration Empty = new CountDuration
    {
        Count = 0,
        Duration = TimeSpan.Zero
    };

    public static CountDuration Create(int count, TimeSpan duration)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(count, 0);

        return new CountDuration
        {
            Count = count,
            Duration = duration
        };
    }

    public static CountDuration operator +(CountDuration left, CountDuration right)
    {
        return new CountDuration
        {
            Count = left.Count + right.Count,
            Duration = left.Duration + right.Duration
        };
    }
}
