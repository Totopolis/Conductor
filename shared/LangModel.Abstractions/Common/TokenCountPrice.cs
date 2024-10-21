namespace LangModel.Abstractions.Common;

public sealed class TokenCountPrice
{
    private TokenCountPrice()
    {
    }

    public required int Count { get; init; }

    public required decimal Price { get; init; }

    public static readonly TokenCountPrice Empty = new TokenCountPrice
    {
        Count = 0,
        Price = 0
    };

    public static TokenCountPrice Create(int count, decimal price)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(count, 0);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(price, 0);

        return new TokenCountPrice
        {
            Count = count,
            Price = price
        };
    }

    public static TokenCountPrice operator +(TokenCountPrice left, TokenCountPrice right)
    {
        return new TokenCountPrice
        {
            Count = left.Count + right.Count,
            Price = left.Price + right.Price
        };
    }
}
