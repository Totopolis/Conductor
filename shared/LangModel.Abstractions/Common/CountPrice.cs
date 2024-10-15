namespace LangModel.Abstractions.Common;

public sealed class CountPrice
{
    private CountPrice()
    {
    }

    public required int Count { get; init; }

    public required decimal Price { get; init; }

    public static readonly CountPrice Empty = new CountPrice
    {
        Count = 0,
        Price = 0
    };

    public static CountPrice Create(int count, decimal price)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(count, 0);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(price, 0);

        return new CountPrice
        {
            Count = count,
            Price = price
        };
    }

    public static CountPrice operator +(CountPrice left, CountPrice right)
    {
        return new CountPrice
        {
            Count = left.Count + right.Count,
            Price = left.Price + right.Price
        };
    }
}
